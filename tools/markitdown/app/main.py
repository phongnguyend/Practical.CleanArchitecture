from fastapi import FastAPI, UploadFile, File, HTTPException
from fastapi.responses import PlainTextResponse
from markitdown import MarkItDown
import tempfile
import os

app = FastAPI(
    title="MarkItDown API",
    description="Convert documents to Markdown",
    version="1.0.0"
)

# Initialize converter once (important for performance)
md = MarkItDown()

# Limits
MAX_FILE_SIZE = 25 * 1024 * 1024  # 25 MB
CHUNK_SIZE = 1024 * 1024  # 1 MB


@app.get("/health")
def health():
    return {"status": "ok"}


@app.post("/convert", response_class=PlainTextResponse)
async def convert(file: UploadFile = File(...)):
    tmp_path = None
    size = 0

    try:
        # Create temp file
        with tempfile.NamedTemporaryFile(delete=False) as tmp:
            tmp_path = tmp.name

            # Stream file to disk in chunks
            while True:
                chunk = await file.read(CHUNK_SIZE)
                if not chunk:
                    break

                size += len(chunk)

                if size > MAX_FILE_SIZE:
                    raise HTTPException(
                        status_code=413,
                        detail="File too large (max 25MB)"
                    )

                tmp.write(chunk)

        # Convert file
        result = md.convert(tmp_path)

        return result.text_content

    finally:
        # Cleanup temp file
        if tmp_path and os.path.exists(tmp_path):
            os.remove(tmp_path)

        await file.close()