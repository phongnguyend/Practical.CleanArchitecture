package com.classifiedads.mobile.features.users

import android.net.Uri
import com.classifiedads.mobile.core.network.Api
import com.classifiedads.mobile.shared.model.RecordRepository
import com.classifiedads.mobile.shared.model.objects
import org.json.JSONArray
import org.json.JSONObject

class UserRepository(val api: Api) : RecordRepository<User> {
    override suspend fun list() = (api.json("users") as JSONArray).objects().map(User::fromJson)

    override suspend fun get(id: String) =
        User.fromJson(api.json("users/${Uri.encode(id)}") as JSONObject)

    override suspend fun remove(id: String) {
        api.json("users/${Uri.encode(id)}", "DELETE")
    }

    override suspend fun save(value: User, file: Uri?) {
        api.json(
            "users" + if (value.id.isEmpty()) "" else "/${Uri.encode(value.id)}",
            if (value.id.isEmpty()) "POST" else "PUT",
            value.toPayload(),
        )
    }

    suspend fun history(id: String) = api.json("users/${Uri.encode(id)}/auditlogs") as JSONArray
}
