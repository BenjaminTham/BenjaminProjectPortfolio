"use client";

import {useState} from "react";
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
import {DeleteTrayDialog} from "./delete-tray-dailog";
import {EditTrayDialog} from "./edit-tray-dailog";

enum Dialogs {
    DELETE,
    EDIT,
}

interface Properties {
    id: number;
    epc: string;
    status: string;
    statusChg: Date;
    rackId: number | null;

}

interface DataTableRowActionsProps<TData extends Properties> {
    row: Row<TData>;
}

export function DataTableRowActions<TData extends Properties>({
                                                                  row,
                                                              }: DataTableRowActionsProps<TData>) {
    const [dialog, setDialog] = useState<Dialogs>(Dialogs.DELETE);

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
                            Copy Tray ID
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
                        ? <DeleteTrayDialog trayId={row.original.id}/>
                        : <EditTrayDialog trayId={row.original.id} rackId={row.original.rackId}/>
                    }
                </DialogContent>
            </Dialog>
        </>
    );
}