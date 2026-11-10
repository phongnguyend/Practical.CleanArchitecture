package com.classifiedads.mobile.features.workspace

import com.classifiedads.mobile.features.configurationentries.ConfigurationEntry
import com.classifiedads.mobile.features.files.StoredFile
import com.classifiedads.mobile.features.products.Product
import com.classifiedads.mobile.features.roles.Role
import com.classifiedads.mobile.features.users.User
import com.classifiedads.mobile.features.users.validatePassword
import com.classifiedads.mobile.shared.model.*
import org.json.JSONObject
import org.junit.Assert.*
import org.junit.Test

class WorkflowTest {
    @Test
    fun passwordsRequireMatchingConfirmation() {
        assertEquals("Passwords do not match.", validatePassword(" secret ", "secret"))
        assertNull(validatePassword(" secret ", " secret "))
    }

    @Test
    fun productAndFileValidationIsFeatureSpecific() {
        assertTrue(Product(name = "ab").validate()!!.contains("at least 3"))
        assertNull(Product(name = "Valid", code = "abc", description = "Example").validate())
        assertTrue(StoredFile(name = "Report").validate()!!.contains("required"))
        assertNull(StoredFile(name = "Report", description = "Example").validate())
    }

    @Test
    fun userPayloadRetainsAccountFields() {
        val user =
            User(
                id = "u1",
                userName = "tester",
                email = " test@example.com ",
                accessFailedCount = 2,
                emailConfirmed = true,
            )
        assertNull(user.validate())
        val body = user.toPayload()
        assertEquals("u1", body.getString("id"))
        assertEquals("test@example.com", body.getString("email"))
        assertEquals(2, body.getInt("accessFailedCount"))
        assertTrue(body.isNull("lockoutEnd"))
        assertTrue(user.copy(accessFailedCount = -1).validate()!!.contains("nonnegative"))
    }

    @Test
    fun sensitiveConfigurationKeepsExactPayload() {
        val value = ConfigurationEntry(key = "Setting", value = "  exact  ", isSensitive = true)
        assertNull(value.validate())
        assertEquals("  exact  ", value.toPayload().getString("value"))
        assertNotNull(Role().validate())
    }

    @Test
    fun filePayloadPreservesImmutableMetadata() {
        val file =
            StoredFile.fromJson(
                JSONObject()
                    .put("id", "f1")
                    .put("name", "Report")
                    .put("description", "Example")
                    .put("fileName", "report.pdf")
                    .put("encrypted", true)
            )
        val body = file.copy(description = "Updated").toPayload()
        assertTrue(body.getBoolean("encrypted"))
        assertEquals("report.pdf", body.getString("fileName"))
    }

    @Test
    fun auditAndApiErrorsHaveReadableValues() {
        assertEquals(
            "Updated",
            auditSnapshot(JSONObject().put("data", JSONObject().put("name", "Updated")))
                .getString("name"),
        )
        assertEquals(
            "Name is required",
            apiError(400, """{"errors":{"Name":["Name is required"]}}"""),
        )
    }
}
