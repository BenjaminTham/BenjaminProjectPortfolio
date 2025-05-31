"use client"

import {CheckCircledIcon, Cross2Icon, CrossCircledIcon} from "@radix-ui/react-icons"
import {Table} from "@tanstack/react-table"
import {Button} from "@/components/ui/button"
import {DataTableViewOptions} from "@/components/data-table/data-table-view-options"
import {DataTableFacetedFilter} from "@/components/data-table/data-table-faceted-filter"
import {DataTableTextFilter} from "@/components/data-table/data-table-text-filter"

const statusesFilterOption = [
    {
        value: "OUT",
        label: "Out",
        icon: CrossCircledIcon
    },
    {
        value: "IN",
        label: "In",
        icon: CheckCircledIcon
    },
]

interface DataTableToolbarProps<TData> {
    table: Table<TData>
}

export function HomeDataTableToolbar<TData>({
                                                table,
                                            }: DataTableToolbarProps<TData>) {
    const isFiltered = table.getState().columnFilters.length > 0
    return (
        <div className="flex items-center justify-between">
            <div className="flex flex-1 items-center space-x-2">
                <DataTableTextFilter table={table} accessorKey="rackId" placeholder="Filter by Rack ID"/>
                {table.getColumn("status") && (
                    <DataTableFacetedFilter
                        column={table.getColumn("status")}
                        title="Status"
                        options={statusesFilterOption}
                    />
                )}
                {isFiltered && (
                    <Button
                        variant="ghost"
                        onClick={() => table.resetColumnFilters()}
                        className="h-8 px-2 lg:px-3"
                    >
                        Reset
                        <Cross2Icon className="ml-2 h-4 w-4"/>
                    </Button>
                )}
            </div>
            <DataTableViewOptions table={table}/>
        </div>
    )
}
