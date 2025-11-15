using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ClassifiedAds.Background.Services;

public class Chunk
{
    public required string Text { get; init; }

    public required int StartIndex { get; init; }

    public required int EndIndex { get; init; }
}

public class TextChunkingService
{
    public static IEnumerable<Chunk> ChunkSentences(string text, int maxTokens = 800)
    {
        // Split text into sentences while preserving their original positions
        var sentenceMatches = Regex.Matches(text, @"[^\.!\?]*[\.!\?]\s*");
        var sentences = new List<(string content, int start, int end)>();

        int lastEnd = 0;
        foreach (Match match in sentenceMatches)
        {
            sentences.Add((match.Value, match.Index, match.Index + match.Length - 1));
            lastEnd = match.Index + match.Length;
        }

        // Handle any remaining text that doesn't end with sentence punctuation
        if (lastEnd < text.Length)
        {
            string remaining = text.Substring(lastEnd);
            if (!string.IsNullOrWhiteSpace(remaining))
            {
                sentences.Add((remaining, lastEnd, text.Length - 1));
            }
        }

        var current = new StringBuilder();
        int tokenCount = 0;
        int chunkStartIndex = -1;
        int chunkEndIndex = -1;

        foreach (var (content, start, end) in sentences)
        {
            int sentenceTokens = content.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;

            if (tokenCount + sentenceTokens > maxTokens && current.Length > 0)
            {
                yield return new Chunk
                {
                    Text = current.ToString().Trim(),
                    StartIndex = chunkStartIndex,
                    EndIndex = chunkEndIndex
                };

                current.Clear();
                tokenCount = 0;
                chunkStartIndex = -1;
            }

            if (current.Length == 0)
            {
                chunkStartIndex = start;
            }

            current.Append(content);
            tokenCount += sentenceTokens;
            chunkEndIndex = end;
        }

        if (current.Length > 0)
        {
            yield return new Chunk
            {
                Text = current.ToString().Trim(),
                StartIndex = chunkStartIndex,
                EndIndex = chunkEndIndex
            };
        }
    }

    public static IEnumerable<Chunk> ChunkSentencesOverlapping(string text, int maxTokens = 800, double overlapRatio = 0.1)
    {
        // Split text into sentences while preserving their original positions
        var sentenceMatches = Regex.Matches(text, @"[^\.!\?]*[\.!\?]\s*");
        var sentences = new List<(string content, int start, int end)>();

        int lastEnd = 0;
        foreach (Match match in sentenceMatches)
        {
            sentences.Add((match.Value.Trim(), match.Index, match.Index + match.Length - 1));
            lastEnd = match.Index + match.Length;
        }

        // Handle remaining text
        if (lastEnd < text.Length)
        {
            string remaining = text.Substring(lastEnd);
            if (!string.IsNullOrWhiteSpace(remaining))
            {
                sentences.Add((remaining.Trim(), lastEnd, text.Length - 1));
            }
        }

        var current = new StringBuilder();
        int tokenCount = 0;
        int estimatedOverlapTokens = (int)(maxTokens * overlapRatio);
        var lastChunkSentences = new List<(string content, int start, int end)>();
        int chunkStartIndex = -1;
        int chunkEndIndex = -1;

        foreach (var sentence in sentences)
        {
            int sentenceTokens = EstimateTokens(sentence.content);

            // If adding this sentence exceeds limit, yield current chunk
            if (tokenCount + sentenceTokens > maxTokens && current.Length > 0)
            {
                yield return new Chunk
                {
                    Text = current.ToString().Trim(),
                    StartIndex = chunkStartIndex,
                    EndIndex = chunkEndIndex
                };

                // Prepare overlap from previous sentences
                var overlapSentences = lastChunkSentences.TakeLast(Math.Max(1, estimatedOverlapTokens / 20)).ToList();
                if (overlapSentences.Any())
                {
                    var overlap = string.Join(" ", overlapSentences.Select(s => s.content));
                    current.Clear();
                    current.Append(overlap + " ");
                    tokenCount = EstimateTokens(overlap);
                    chunkStartIndex = overlapSentences.First().start;
                }
                else
                {
                    current.Clear();
                    tokenCount = 0;
                    chunkStartIndex = sentence.start;
                }
                lastChunkSentences.Clear();
            }

            if (current.Length == 0)
            {
                chunkStartIndex = sentence.start;
            }

            current.Append(sentence.content + " ");
            tokenCount += sentenceTokens;
            chunkEndIndex = sentence.end;
            lastChunkSentences.Add(sentence);
        }

        if (current.Length > 0)
        {
            yield return new Chunk
            {
                Text = current.ToString().Trim(),
                StartIndex = chunkStartIndex,
                EndIndex = chunkEndIndex
            };
        }
    }

    private static int EstimateTokens(string text)
    {
        // Rough heuristic: 1 token ≈ 4 chars
        return Math.Max(1, text.Length / 4);
    }
}
