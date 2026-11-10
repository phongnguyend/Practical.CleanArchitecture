import { useState } from "react";
import { Text } from "react-native";
import { auditSnapshot } from "../../shared/models/record";
import { ImportButton } from "../../shared/ui/ImportButton";
import { ProductImage, Rating } from "../../shared/ui/record-ui";
import { RecordPage } from "../../shared/ui/RecordPage";
import { Button } from "../../shared/ui/ui";
import * as service from "./api";
import { emptyProduct, parseProduct, validateProduct } from "./model";
import {
  ProductDetails,
  ProductHistory,
  ProductSummary,
} from "./ProductDetails";
import { ProductForm } from "./ProductForm";
export default function ProductScreen() {
  const [showImages, setShowImages] = useState(false);
  const [ratingMessage, setRatingMessage] = useState("");
  return (
    <RecordPage
      title="Products"
      repository={service.productsRepository}
      empty={emptyProduct}
      label={(value) => value.name}
      searchText={(value) => value.name}
      validate={validateProduct}
      Form={ProductForm}
      Details={ProductDetails}
      Summary={({ value }) => (
        <>
          {showImages && <ProductImage row={value} />}
          <ProductSummary value={value} />
          <Rating
            value={value.starRating || 4}
            onPress={() =>
              setRatingMessage(
                `The rating ${value.starRating || 4} was clicked!`,
              )
            }
          />
          {!!ratingMessage && <Text>{ratingMessage}</Text>}
        </>
      )}
      History={(entry) => (
        <ProductHistory
          value={parseProduct(auditSnapshot(entry))}
          changed={entry.highLight}
        />
      )}
      toolbar={(actions) => (
        <>
          <Button
            title={showImages ? "Hide Image" : "Show Image"}
            onPress={() => setShowImages(!showImages)}
          />
          <ImportButton
            suffix=".csv"
            submit={service.importRecords}
            done={async () => {
              await actions.reload();
              actions.notify("Import processed");
            }}
          />
          <Button
            title="Export CSV"
            onPress={() => void actions.run(service.exportascsv)}
          />
          <Button
            title="Export PDF"
            onPress={() => void actions.run(service.exportaspdf)}
          />
        </>
      )}
    />
  );
}
