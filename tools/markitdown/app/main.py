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

def positive_int_env(name: str, default: int) -> int:
    try:
        value = int(os.environ.get(name, str(default)))
    except ValueError:
        raise ValueError(f"{name} must be a positive integer (MB)") from None
    if value <= 0:
        raise ValueError(f"{name} must be a positive integer (MB)")
    return value


# Environment values are in MB (1 MB = 1024 * 1024 bytes).
# Convert to bytes for upload size checks and streaming.
MAX_FILE_SIZE = positive_int_env("MAX_FILE_SIZE_MB", 50) * 1024 * 1024
CHUNK_SIZE = positive_int_env("CHUNK_SIZE_MB", 1) * 1024 * 1024


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
                        detail=f"File too large (max {MAX_FILE_SIZE // (1024 * 1024)} MB)"
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
