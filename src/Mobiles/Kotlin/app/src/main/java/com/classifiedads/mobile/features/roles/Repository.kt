package com.classifiedads.mobile.features.roles

import android.net.Uri
import com.classifiedads.mobile.core.network.Api
import com.classifiedads.mobile.shared.model.RecordRepository
import com.classifiedads.mobile.shared.model.objects
import org.json.JSONArray
import org.json.JSONObject

class RoleRepository(val api: Api) : RecordRepository<Role> {
    override suspend fun list() = (api.json("roles") as JSONArray).objects().map(Role::fromJson)

    override suspend fun get(id: String) =
        Role.fromJson(api.json("roles/${Uri.encode(id)}") as JSONObject)

    override suspend fun remove(id: String) {
        api.json("roles/${Uri.encode(id)}", "DELETE")
    }

    override suspend fun save(value: Role, file: Uri?) {
        api.json(
            "roles" + if (value.id.isEmpty()) "" else "/${Uri.encode(value.id)}",
            if (value.id.isEmpty()) "POST" else "PUT",
            value.toPayload(),
        )
    }
}
