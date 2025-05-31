import {DialogClose, DialogDescription, DialogFooter, DialogHeader, DialogTitle} from "@/components/ui/dialog";
import {Button} from "@/components/ui/button";

import {toast} from "@/components/ui/use-toast";
import {useRouter} from "next/navigation"
import {deleteTray} from "@/domain/trayControl";

interface DeleteTrayDialogProps {
    trayId: number;
}

export function DeleteTrayDialog({trayId}: DeleteTrayDialogProps) {
    const router = useRouter();

    const handleDelete = async () => {
        try {
            await deleteTray(trayId)
            toast({
                title: "Selected tray has been deleted"
            });
            router.refresh(); // Refresh the data table to reflect changes
        } catch (error) {
            console.error(error);
            toast({
                title: "Unable to delete selected tray",
            });
        }
    };

    return (
        <>
            <DialogHeader>
                <DialogTitle>Are you absolutely sure?</DialogTitle>
                <DialogDescription>
                    This action cannot be undone. This will remove the tray {trayId} from the database.
                </DialogDescription>
            </DialogHeader>
            <DialogFooter>
                <DialogClose asChild>
                    <Button variant="secondary">
                        Close
                    </Button>
                </DialogClose>
                <DialogClose asChild>
                    <Button onClick={handleDelete}>Continue</Button>
                </DialogClose>
            </DialogFooter>
        </>
    );
}