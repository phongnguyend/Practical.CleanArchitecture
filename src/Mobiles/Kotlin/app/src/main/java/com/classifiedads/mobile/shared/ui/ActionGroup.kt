package com.classifiedads.mobile.shared.ui

import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.runtime.Composable
import androidx.compose.ui.Modifier
import androidx.compose.ui.layout.Layout
import androidx.compose.ui.unit.Constraints
import androidx.compose.ui.unit.dp

/** Equal action columns; navigation tabs deliberately keep their natural width. */
@Composable
fun ActionGroup(content: @Composable () -> Unit) {
    Layout(content = content, modifier = Modifier.fillMaxWidth()) { children, constraints ->
        val gap = 12.dp.roundToPx()
        val minimumCellWidth = (144.dp.toPx() * fontScale).toInt()
        val columns = if (constraints.maxWidth >= minimumCellWidth * 2 + gap) {
            2
        } else {
            1
        }
        val cellWidth = (constraints.maxWidth - gap * (columns - 1)) / columns
        val rows = children.chunked(columns).map { row ->
            val rowHeight = row.maxOf { it.maxIntrinsicHeight(cellWidth) }.coerceAtLeast(48.dp.roundToPx())
            row.map { child -> child.measure(Constraints.fixed(cellWidth, rowHeight)) }
        }
        val height = rows.sumOf { it.first().height } + gap * (rows.size - 1).coerceAtLeast(0)
        layout(constraints.maxWidth, height) {
            var top = 0
            for (row in rows) {
                row.forEachIndexed { column, child ->
                    child.placeRelative(column * (cellWidth + gap), top)
                }
                top += row.first().height + gap
            }
        }
    }
}
