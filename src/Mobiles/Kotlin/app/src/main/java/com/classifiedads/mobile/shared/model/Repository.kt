package com.classifiedads.mobile.shared.model

import android.net.Uri

interface Entity {
    val id: String
}

interface RecordRepository<T : Entity> {
    suspend fun list(): List<T>

    suspend fun get(id: String): T

    suspend fun save(value: T, file: Uri?)

    suspend fun remove(id: String)
}
