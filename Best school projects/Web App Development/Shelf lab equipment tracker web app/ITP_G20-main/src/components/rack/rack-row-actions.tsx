"use client";

import {Row} from "@tanstack/react-table";
import {Button} from "@/components/ui/button";

import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuLabel,
    DropdownMenuSeparator,
    DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import {MoreHorizontal} from "lucide-react";
import {Dialog, DialogContent, DialogTrigger} from "@/components/ui/dialog";
import {useState} from "react";
import {DeleteRackDialog} from "@/components/rack/delete-rack-dailog";
import {EditRackDialog} from "@/components/rack/edit-rack-dailog";

interface DataTableRowActionsProps<TData extends { id: number, location: string, name: string }> {
    row: Row<TData>;
}

enum Dialogs {
    DELETE,
    EDIT,
}

export function DataTableRowActions<TData extends { id: number, location: string, name: string }>({
                                                                      row,
                                                                  }: DataTableRowActionsProps<TData>) {
    const [dialog, setDialog] = useState<Dialogs>(Dialogs.DELETE);
    const rackId = row.original.id;

    return (
        <>
            <Dialog>
                <DropdownMenu>
                    <DropdownMenuTrigger asChild>
                        <Button variant="ghost" className="h-8 w-8 p-0">
                            <span className="sr-only">Open menu</span>
                            <MoreHorizontal className="h-4 w-4"/>
                        </Button>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent align="end">
                        <DropdownMenuLabel>Actions</DropdownMenuLabel>
                        <DropdownMenuItem
                            onClick={() => navigator.clipboard.writeText(row.original.id.toString())}
                        >
                            Copy Rack ID
                        </DropdownMenuItem>
                        <DropdownMenuSeparator/>
                        <DialogTrigger
                            asChild
                            onClick={() => {
                                setDialog(Dialogs.DELETE)
                            }}
                        >
                            <DropdownMenuItem>
                                Delete
                            </DropdownMenuItem>
                        </DialogTrigger>
                        <DialogTrigger
                            asChild
                            onClick={() => {
                                setDialog(Dialogs.EDIT)
                            }}
                        >
                            <DropdownMenuItem>
                                Edit
                            </DropdownMenuItem>
                        </DialogTrigger>
                    </DropdownMenuContent>
                </DropdownMenu>
                <DialogContent>
                    {dialog === Dialogs.DELETE
                        ? <DeleteRackDialog rackId={rackId}/>
                        : <EditRackDialog rackId={rackId} location={row.original.location} name={row.original.name}/>
                    }
                </DialogContent>
            </Dialog>
        </>
    );
}

