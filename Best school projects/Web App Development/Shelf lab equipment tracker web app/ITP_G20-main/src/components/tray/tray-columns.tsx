"use client"

import {ColumnDef} from "@tanstack/react-table"
import {Tray} from "@prisma/client"
import {DataTableColumnHeader} from "@/components/data-table/data-table-column-header"
import {DataTableRowActions} from "@/components/tray/tray-row-actions"
import {Checkbox} from "@/components/ui/checkbox"

export const trayColumns: ColumnDef<Tray>[] = [
    {
        id: "select",
        header: ({table}) => (
            <Checkbox
                checked={
                    table.getIsAllPageRowsSelected() ||
                    (table.getIsSomePageRowsSelected() && "indeterminate")
                }
                onCheckedChange={(value) => table.toggleAllPageRowsSelected(!!value)}
                aria-label="Select all"
            />
        ),
        cell: ({row}) => (
            <Checkbox
                checked={row.getIsSelected()}
                onCheckedChange={(value) => row.toggleSelected(!!value)}
                aria-label="Select row"
            />
        ),
    },
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
        header: "Status",
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
    },
    {
        id: "actions",
        cell: ({row}) => {
            const tray = row.original.id

            return (
                <DataTableRowActions row={row}/>
            )
        },
    }
]
