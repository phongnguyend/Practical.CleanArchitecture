package com.classifiedads.mobile.features.products

import androidx.compose.foundation.layout.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.platform.LocalContext
import com.classifiedads.mobile.core.network.Api
import com.classifiedads.mobile.shared.model.auditSnapshot
import com.classifiedads.mobile.shared.ui.*
import org.json.JSONObject

@Composable
fun ProductScreen(api: Api) {
    val repository = remember(api) { ProductRepository(api) }
    val context = LocalContext.current
    var showImages by remember { mutableStateOf(false) }
    var ratingMessage by remember { mutableStateOf("") }
    RecordPage(
        title = "Products",
        repository = repository,
        empty = { Product() },
        label = { it.name },
        searchText = { it.name },
        serialize = { it.toJson() },
        validate = { it.validate() },
        form = { value, change, file, pick, busy ->
            ProductForm(value, change, file, pick, busy, api::fileName)
        },
        summary = { value ->
            if (showImages) ProductPicture(value.toJson())
            ProductSummary(value)
            OutlinedButton(shape = MaterialTheme.shapes.small,
                onClick = {
                    ratingMessage =
                        "The rating ${if(value.starRating==0.0)4.0 else value.starRating} was clicked!"
                }
            ) {
                Text("★ ${if(value.starRating==0.0)4.0 else value.starRating}/5")
            }
            if (ratingMessage.isNotEmpty()) Text(ratingMessage)
        },
        details = { ProductDetails(it) },
        history = repository::history,
        historyBody = { entry ->
            ProductHistory(
                Product.fromJson(auditSnapshot(entry)),
                entry.optJSONObject("highLight") ?: JSONObject(),
            )
        },
        toolbar = { reload ->
            OutlinedButton(shape = MaterialTheme.shapes.small, onClick = { showImages = !showImages }) {
                ActionLabel(if (showImages) "Hide Image" else "Show Image")
            }
            ImportButton(".csv", api::fileName, repository::importFile, reload)
            OperationButton("Export CSV") {
                context.startActivity(api.download("products/exportascsv", "Products.csv"))
            }
            OperationButton("Export PDF") {
                context.startActivity(api.download("products/exportaspdf", "Products.pdf"))
            }
        },
    )
}
