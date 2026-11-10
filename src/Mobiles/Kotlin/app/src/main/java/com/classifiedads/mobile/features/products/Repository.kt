package com.classifiedads.mobile.features.products

import android.net.Uri
import com.classifiedads.mobile.core.network.Api
import com.classifiedads.mobile.shared.model.RecordRepository
import com.classifiedads.mobile.shared.model.objects
import org.json.JSONArray
import org.json.JSONObject

class ProductRepository(val api: Api) : RecordRepository<Product> {
    override suspend fun list() =
        (api.json("products") as JSONArray).objects().map(Product::fromJson)

    override suspend fun get(id: String) =
        Product.fromJson(api.json("products/${Uri.encode(id)}") as JSONObject)

    override suspend fun remove(id: String) {
        api.json("products/${Uri.encode(id)}", "DELETE")
    }

    override suspend fun save(value: Product, file: Uri?) {
        api.json(
            "products" + if (value.id.isEmpty()) "" else "/${Uri.encode(value.id)}",
            if (value.id.isEmpty()) "POST" else "PUT",
            value.toPayload(),
        )
    }

    suspend fun history(id: String) = api.json("products/${Uri.encode(id)}/auditlogs") as JSONArray

    suspend fun importFile(file: Uri) {
        api.upload("products/importcsv", file)
    }
}
