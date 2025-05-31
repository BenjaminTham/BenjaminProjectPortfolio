"use client"

import {Cross2Icon, QuestionMarkCircledIcon} from "@radix-ui/react-icons"
import {Table} from "@tanstack/react-table"

import {Button} from "../ui/button"
import {DataTableViewOptions} from "../data-table/data-table-view-options"

import {DataTableFacetedFilter} from "../data-table/data-table-faceted-filter"
import {DataTableTextFilter} from "../data-table/data-table-text-filter"
import {deleteTray} from "@/domain/trayControl"
import {toast} from "../ui/use-toast"
import {useRouter} from "next/navigation"
import {
    Dialog,
    DialogClose,
    DialogContent,
    DialogDescription,
    DialogFooter,
    DialogHeader,
    DialogTitle,
    DialogTrigger
} from "../ui/dialog"

const statuses = [
    {
        value: "OUT",
        label: "Out",
        icon: QuestionMarkCircledIcon,
    },
    {
        value: "IN",
        label: "In",
        icon: QuestionMarkCircledIcon,
    },
]

interface Properties {
    id: number
}

interface DataTableToolbarProps<TData extends Properties> {
    table: Table<TData>
}

export function DataTableToolbar<TData extends Properties>({
                                                               table,
                                                           }: DataTableToolbarProps<TData>) {
    const isFiltered = table.getState().columnFilters.length > 0
    const router = useRouter();
    const selectedRows = table.getFilteredSelectedRowModel().rows.map(row => row.original.id);

    const handleDelete = async () => {
        try {
            await Promise.all(Array.from(selectedRows).map(id =>
                deleteTray(id)
            ));
            toast({
                title: "Selected trays have been deleted"
            });
            router.refresh(); // Refresh the data table to reflect changes
        } catch (error) {
            console.error(error);
            toast({
                title: "Unable to delete selected trays",
            });
        }
    };

    return (
        <>
            <div className="flex items-center justify-between">
                <div className="flex flex-1 items-center space-x-2">
                    <DataTableTextFilter table={table} accessorKey="id" placeholder="Filter by ID"/>
                    {table.getColumn("status") && (
                        <DataTableFacetedFilter
                            column={table.getColumn("status")}
                            title="Status"
                            options={statuses}
                        />
                    )}
                    <Dialog>
                        {selectedRows.length > 0 && (
                            <DialogTrigger>
                                <Button variant="destructive" className="h-8">
                                    Delete {selectedRows.length} selected trays
                                </Button>
                            </DialogTrigger>
                        )}
                        <DialogContent>
                            <DialogHeader>
                                <DialogTitle>Are you absolutely sure?</DialogTitle>
                                <DialogDescription>
                                    This action cannot be undone. This will remove the selected trays from the database.
                                    <ul className="my-6 ml-6 list-disc [&>li]:mt-2">
                                        {selectedRows.map((id) => (
                                            <li key={id} className="text-sm text-gray-700">
                                                Tray ID: {id}
                                            </li>
                                        ))}
                                    </ul>
                                </DialogDescription>
                            </DialogHeader>
                            <DialogFooter>
                                <DialogClose asChild>
                                    <Button type="button" variant="secondary">
                                        Close
                                    </Button>
                                </DialogClose>
                                <DialogClose asChild>
                                    <Button onClick={handleDelete}>Continue</Button>
                                </DialogClose>
                            </DialogFooter>
                        </DialogContent>
                    </Dialog>
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
        </>
    )
}
