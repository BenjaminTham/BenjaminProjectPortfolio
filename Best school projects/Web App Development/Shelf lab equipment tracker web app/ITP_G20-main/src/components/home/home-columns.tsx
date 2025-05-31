"use client"

import {ColumnDef} from "@tanstack/react-table"
import {Tray} from "@prisma/client"
import {DataTableColumnHeader} from "@/components/data-table/data-table-column-header"

export const homeColumns: ColumnDef<Tray>[] = [
    {
        accessorKey: "id",
        meta: {header: "Tray ID"},
        header: ({column}) => (
            <DataTableColumnHeader column={column} title="Tray ID"/>
        ),
    },
    {
        accessorKey: "epc",
        meta: {header: "Tray EPC"},
        header: ({column}) => (
            <DataTableColumnHeader column={column} title="Tray EPC"/>
        )
    },
    {
        accessorKey: "status",
        meta: {header: "Status"},
        header: ({column}) => (
            <DataTableColumnHeader column={column} title="Status"/>
        ),
        filterFn: (row, id, value) => {
            return value.includes(row.getValue(id))
        },
    },
    {
        accessorKey: "statusChg",
        meta: {header: "Status Change Time"},
        header: ({column}) => (
            <DataTableColumnHeader column={column} title="Status Change Time"/>
        ),
        cell: ({row}) => {
            const tray = row.original
            if (!tray.statusChg) return "N/A"
            const date = new Date(tray.statusChg);
            return date.toLocaleString('en-US', {timeZone: 'Asia/Singapore'})
        }
    },
    {
        accessorKey: "rackId",
        meta: {header: "Rack ID"},
        header: ({column}) => (
            <DataTableColumnHeader column={column} title="Rack ID"/>
        ),
    }
]