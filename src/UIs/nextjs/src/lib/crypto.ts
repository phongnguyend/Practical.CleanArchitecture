/**
 * Encryption utilities for securing sensitive data like access tokens
 * Uses AES-256-GCM encryption with a secret key from environment variables
 */

const ALGORITHM = "AES-GCM";
const KEY_LENGTH = 256;
const IV_LENGTH = 12; // 96 bits recommended for GCM

/**
 * Gets the encryption key from environment variable
 * If not set, returns a default key (should be changed in production)
 */
function getEncryptionKey(): string {
  const key = process.env.ENCRYPTION_SECRET;

  if (!key) {
    console.warn(
      "ENCRYPTION_SECRET not set in environment variables. Using default key. This is insecure for production!"
    );
    // Default key - MUST be changed in production
    return "change-this-to-a-secure-random-32-character-key!!";
  }

  // Ensure key is at least 32 characters (256 bits)
  if (key.length < 32) {
    throw new Error("ENCRYPTION_SECRET must be at least 32 characters long");
  }

  return key;
}

/**
 * Derives a CryptoKey from the secret string
 */
async function deriveKey(secret: string): Promise<CryptoKey> {
  const encoder = new TextEncoder();
  const keyMaterial = encoder.encode(secret.padEnd(32, "0").substring(0, 32));

  // Import the raw key material
  const importedKey = await crypto.subtle.importKey(
    "raw",
    keyMaterial,
    { name: ALGORITHM },
    false,
    ["encrypt", "decrypt"]
  );

  return importedKey;
}

/**
 * Encrypts a string value
 * @param plaintext - The string to encrypt
 * @returns Base64-encoded encrypted string with IV prepended
 */
export async function encrypt(plaintext: string): Promise<string> {
  try {
    const secret = getEncryptionKey();
    const key = await deriveKey(secret);

    // Generate a random IV
    const iv = crypto.getRandomValues(new Uint8Array(IV_LENGTH));

    // Encrypt the plaintext
    const encoder = new TextEncoder();
    const encodedPlaintext = encoder.encode(plaintext);

    const ciphertext = await crypto.subtle.encrypt(
      {
        name: ALGORITHM,
        iv: iv,
      },
      key,
      encodedPlaintext
    );

    // Combine IV and ciphertext
    const combined = new Uint8Array(iv.length + ciphertext.byteLength);
    combined.set(iv, 0);
    combined.set(new Uint8Array(ciphertext), iv.length);

    // Convert to base64
    return Buffer.from(combined).toString("base64");
  } catch (error) {
    console.error("Encryption error:", error);
    throw new Error("Failed to encrypt data");
  }
}

/**
 * Decrypts an encrypted string
 * @param encryptedData - Base64-encoded encrypted string with IV prepended
 * @returns The decrypted plaintext string
 */
export async function decrypt(encryptedData: string): Promise<string> {
  try {
    const secret = getEncryptionKey();
    const key = await deriveKey(secret);

    // Decode from base64
    const combined = Buffer.from(encryptedData, "base64");

    // Extract IV and ciphertext
    const iv = combined.slice(0, IV_LENGTH);
    const ciphertext = combined.slice(IV_LENGTH);

    // Decrypt
    const decrypted = await crypto.subtle.decrypt(
      {
        name: ALGORITHM,
        iv: iv,
      },
      key,
      ciphertext
    );

    // Convert back to string
    const decoder = new TextDecoder();
    return decoder.decode(decrypted);
  } catch (error) {
    console.error("Decryption error:", error);
    throw new Error("Failed to decrypt data");
  }
}

/**
 * Safely checks if an encrypted token exists and is valid
 * @param encryptedData - The encrypted data to check
 * @returns true if data exists and can be decrypted, false otherwise
 */
export async function isValidEncryptedToken(
  encryptedData: string | undefined
): Promise<boolean> {
  if (!encryptedData) {
    return false;
  }

  try {
    const decrypted = await decrypt(encryptedData);
    return !!decrypted;
  } catch {
    return false;
  }
}
