import { Detail } from "../../shared/ui/fields";
import { ProductImage } from "../../shared/ui/record-ui";
import { Product } from "./model";
export function ProductSummary({
  value,
  changed = {},
}: {
  value: Product;
  changed?: Record<string, boolean>;
}) {
  return (
    <>
      <Detail
        label="Code"
        value={value.code.toUpperCase()}
        changed={!!changed.code}
      />
      <Detail
        label="Description"
        value={value.description}
        changed={!!changed.description}
      />
      <Detail
        label="Price"
        value={value.price || 5}
        changed={!!changed.price}
      />
      <Detail
        label="5 Star Rating"
        value={value.starRating || 4}
        changed={!!changed.starRating}
      />
    </>
  );
}
export function ProductDetails({
  value,
  changed = {},
}: {
  value: Product;
  changed?: Record<string, boolean>;
}) {
  return (
    <>
      <ProductImage row={value} />
      <Detail label="Name" value={value.name} changed={!!changed.name} />
      <Detail
        label="Code"
        value={value.code.toUpperCase()}
        changed={!!changed.code}
      />
      <Detail
        label="Description"
        value={value.description}
        changed={!!changed.description}
      />
      <Detail
        label="Price"
        value={value.price || 5}
        changed={!!changed.price}
      />
      <Detail
        label="5 Star Rating"
        value={value.starRating || 4}
        changed={!!changed.starRating}
      />
    </>
  );
}
export function ProductHistory({
  value,
  changed = {},
}: {
  value: Product;
  changed?: Record<string, boolean>;
}) {
  return (
    <>
      <Detail label="Code" value={value.code} changed={!!changed.code} />
      <Detail label="Name" value={value.name} changed={!!changed.name} />
      <Detail
        label="Description"
        value={value.description}
        changed={!!changed.description}
      />
    </>
  );
}
