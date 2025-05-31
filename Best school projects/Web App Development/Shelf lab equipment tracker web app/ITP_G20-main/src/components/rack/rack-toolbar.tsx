"use client"

import {Button} from '@/components/ui/button'
import {createNewRack, deleteRack} from '@/domain/rackControl'
import {Cross2Icon} from "@radix-ui/react-icons"
import {Table} from "@tanstack/react-table"
import {DataTableViewOptions} from "@/components/data-table/data-table-view-options"
import {toast} from "@/components/ui/use-toast"
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
} from "@/components/ui/dialog"
import {z} from 'zod'
import {newRackSchema} from '@/schema/custom'
import {useForm} from 'react-hook-form'
import {zodResolver} from '@hookform/resolvers/zod'
import {Form, FormControl, FormDescription, FormField, FormItem, FormLabel, FormMessage} from '@/components/ui/form'
import {Input} from '@/components/ui/input'

interface Properties {
    id: number
}

interface DataTableToolbarProps<TData extends Properties> {
    table: Table<TData>
}

type CreateRackFormValues = z.infer<typeof newRackSchema>

export function DataTableToolbar<TData extends Properties>({
                                                               table,
                                                           }: DataTableToolbarProps<TData>) {
    const isFiltered = table.getState().columnFilters.length > 0
    const router = useRouter();
    const selectedRows = table.getFilteredSelectedRowModel().rows.map(row => row.original.id);
    const form = useForm<CreateRackFormValues>({
        resolver: zodResolver(newRackSchema),
        defaultValues: {
            location: "",
            name: "",
        },
    });

    const handleCreate = async (data: CreateRackFormValues) => {
        try {
            await createNewRack(data);
            toast({
                title: "Rack has been added"
            });
            router.refresh(); // Refresh the data table to reflect changes
        } catch (error) {
            console.error(error);
            toast({
                title: "Unable to add rack",
            });
        }
    }

    const handleDelete = async () => {
        try {
            await Promise.all(Array.from(selectedRows).map(id =>
                deleteRack(id)
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
                    <Dialog>
                        <DialogTrigger asChild>
                            <Button className="h-8">
                                Add Rack
                            </Button>
                        </DialogTrigger>
                        <DialogContent>
                            <DialogHeader>
                                <DialogTitle>Add Rack</DialogTitle>
                            </DialogHeader>
                            <Form {...form}>
                                <form onSubmit={form.handleSubmit(handleCreate)}>
                                    <FormField
                                        control={form.control}
                                        name="location"
                                        render={({field}) => (
                                            <FormItem>
                                                <FormLabel>Location</FormLabel>
                                                <FormControl>
                                                    <Input placeholder="Location" {...field} />
                                                </FormControl>
                                                <FormDescription>
                                                    The location of the rack.
                                                </FormDescription>
                                                <FormMessage/>
                                            </FormItem>
                                        )}
                                    />
                                    <FormField
                                        control={form.control}
                                        name="name"
                                        render={({field}) => (
                                            <FormItem>
                                                <FormLabel>Name</FormLabel>
                                                <FormControl>
                                                    <Input placeholder="Name" {...field} />
                                                </FormControl>
                                                <FormDescription>
                                                    The name of the rack.
                                                </FormDescription>
                                                <FormMessage/>
                                            </FormItem>
                                        )}
                                    />
                                    <DialogFooter>
                                        <DialogClose asChild>
                                            <Button type="button" variant="secondary">
                                                Close
                                            </Button>
                                        </DialogClose>
                                        <DialogClose asChild>
                                            <Button type="submit">
                                                Save
                                            </Button>
                                        </DialogClose>
                                    </DialogFooter>
                                </form>
                            </Form>
                        </DialogContent>
                    </Dialog>
                    <Dialog>
                        {selectedRows.length > 0 && (
                            <DialogTrigger>
                                <Button variant="destructive" className="h-8">
                                    Delete {selectedRows.length} selected racks.
                                </Button>
                            </DialogTrigger>
                        )}
                        <DialogContent>
                            <DialogHeader>
                                <DialogTitle>Are you absolutely sure?</DialogTitle>
                                <DialogDescription>
                                    This action cannot be undone. This will remove the selected racks from the database.
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
                                    <Button onClick={handleDelete}>
                                        Continue
                                    </Button>
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
