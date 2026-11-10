package com.classifiedads.mobile.features.configurationentries

import android.net.Uri
import com.classifiedads.mobile.core.network.Api
import com.classifiedads.mobile.shared.model.RecordRepository
import com.classifiedads.mobile.shared.model.objects
import org.json.JSONArray
import org.json.JSONObject

class ConfigurationEntryRepository(val api: Api) : RecordRepository<ConfigurationEntry> {
    override suspend fun list() =
        (api.json("configurationentries") as JSONArray).objects().map(ConfigurationEntry::fromJson)

    override suspend fun get(id: String) =
        ConfigurationEntry.fromJson(
            api.json("configurationentries/${Uri.encode(id)}") as JSONObject
        )

    override suspend fun remove(id: String) {
        api.json("configurationentries/${Uri.encode(id)}", "DELETE")
    }

    override suspend fun save(value: ConfigurationEntry, file: Uri?) {
        api.json(
            "configurationentries" + if (value.id.isEmpty()) "" else "/${Uri.encode(value.id)}",
            if (value.id.isEmpty()) "POST" else "PUT",
            value.toPayload(),
        )
    }

    suspend fun importFile(file: Uri) {
        api.upload("configurationentries/ImportExcel", file)
    }
}
