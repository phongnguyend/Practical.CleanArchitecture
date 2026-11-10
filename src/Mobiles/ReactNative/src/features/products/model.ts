export interface Product {
  id: string;
  name: string;
  code: string;
  description: string;
  price: number;
  starRating: number;
  imageUrl: string;
}
export const emptyProduct = (): Product => ({
  id: "",
  name: "",
  code: "",
  description: "",
  price: 0,
  starRating: 0,
  imageUrl: "",
});
export function parseProduct(value: Record<string, unknown>): Product {
  return {
    id: String(value.id ?? ""),
    name: String(value.name ?? ""),
    code: String(value.code ?? ""),
    description: String(value.description ?? ""),
    price: Number(value.price ?? 0),
    starRating: Number(value.starRating ?? 0),
    imageUrl: String(value.imageUrl ?? ""),
  };
}
export function validateProduct(value: Product): string | undefined {
  if (!value.name.trim()) return "Name is required.";
  if (value.name.trim().length < 3) return "Name needs at least 3 characters.";
  if (!value.code.trim()) return "Code is required.";
  if (value.code.length > 10) return "Code must be at most 10 characters.";
  if (!value.description.trim()) return "Description is required.";
  if (value.description.length > 100)
    return "Description must be at most 100 characters.";
}
export function productsPayload(value: Product) {
  return {
    ...(value.id ? { id: value.id } : {}),
    name: value.name,
    code: value.code,
    description: value.description,
  };
}
