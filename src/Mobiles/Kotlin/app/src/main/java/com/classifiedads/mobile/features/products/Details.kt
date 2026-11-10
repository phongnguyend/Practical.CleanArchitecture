package com.classifiedads.mobile.features.products

import androidx.compose.foundation.layout.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import com.classifiedads.mobile.shared.ui.*
import org.json.JSONObject

@Composable
fun ProductSummary(value: Product, changed: JSONObject = JSONObject()) {
    RecordValue("Code", value.code.uppercase(), changed.optBoolean("code"))
    RecordValue("Description", value.description, changed.optBoolean("description"))
    RecordValue("Price", if (value.price == 0.0) 5.0 else value.price, changed.optBoolean("price"))
    RecordValue(
        "5 Star Rating",
        if (value.starRating == 0.0) 4.0 else value.starRating,
        changed.optBoolean("starRating"),
    )
}

@Composable
fun ProductDetails(value: Product, changed: JSONObject = JSONObject()) {
    ProductPicture(value.toJson())
    RecordValue("Name", value.name, changed.optBoolean("name"))
    RecordValue("Code", value.code.uppercase(), changed.optBoolean("code"))
    RecordValue("Description", value.description, changed.optBoolean("description"))
    RecordValue("Price", if (value.price == 0.0) 5.0 else value.price, changed.optBoolean("price"))
    RecordValue(
        "5 Star Rating",
        if (value.starRating == 0.0) 4.0 else value.starRating,
        changed.optBoolean("starRating"),
    )
}

@Composable
fun ProductHistory(value: Product, changed: JSONObject = JSONObject()) {
    RecordValue("Code", value.code, changed.optBoolean("code"))
    RecordValue("Name", value.name, changed.optBoolean("name"))
    RecordValue("Description", value.description, changed.optBoolean("description"))
}
