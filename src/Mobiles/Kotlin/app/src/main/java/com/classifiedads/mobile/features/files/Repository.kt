package com.classifiedads.mobile.features.files

import android.net.Uri
import com.classifiedads.mobile.core.network.Api
import com.classifiedads.mobile.shared.model.RecordRepository
import com.classifiedads.mobile.shared.model.objects
import org.json.JSONArray
import org.json.JSONObject

class StoredFileRepository(val api: Api) : RecordRepository<StoredFile> {
    override suspend fun list() =
        (api.json("files") as JSONArray).objects().map(StoredFile::fromJson)

    override suspend fun get(id: String) =
        StoredFile.fromJson(api.json("files/${Uri.encode(id)}") as JSONObject)

    override suspend fun remove(id: String) {
        api.json("files/${Uri.encode(id)}", "DELETE")
    }

    override suspend fun save(value: StoredFile, file: Uri?) {
        if (value.id.isEmpty()) {
            api.upload("files", requireNotNull(file) { "Choose a file." }, value.toPayload())
            return
        }
        api.json(
            "files" + if (value.id.isEmpty()) "" else "/${Uri.encode(value.id)}",
            if (value.id.isEmpty()) "POST" else "PUT",
            value.toPayload(),
        )
    }

    suspend fun history(id: String) = api.json("files/${Uri.encode(id)}/auditlogs") as JSONArray
}
