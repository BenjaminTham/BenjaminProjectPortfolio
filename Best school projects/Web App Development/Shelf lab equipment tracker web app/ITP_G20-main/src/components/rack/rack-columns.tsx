"use client"

import {ColumnDef} from "@tanstack/react-table";
import {DataTableColumnHeader} from "@/components/data-table/data-table-column-header";
import {Checkbox} from "@/components/ui/checkbox";
import {DataTableRowActions} from "@/components/rack/rack-row-actions";
import {Rack} from "@prisma/client";

export const rackColumns: ColumnDef<Rack>[] = [
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
        meta: {header: "Rack ID"},
        header: ({column}) => (
            <DataTableColumnHeader column={column} title="Rack ID"/>
        ),
    },
    {
        accessorKey: "location",
        meta: {header: "Location"},
        header: ({column}) => (
            <DataTableColumnHeader column={column} title="Location"/>
        ),
    },
    {
        accessorKey: "name",
        meta: {header: "Rack Name"},
        header: ({column}) => (
            <DataTableColumnHeader column={column} title="Rack Name"/>
        ),
    },
    {
        id: "actions",
        cell: ({row}) => {
            const rack = row.original.id

            return (
                <DataTableRowActions row={row}/>
            )
        },
    }
];
