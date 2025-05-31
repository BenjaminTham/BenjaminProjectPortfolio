import {DialogClose, DialogDescription, DialogFooter, DialogHeader, DialogTitle} from "@/components/ui/dialog";
import {Button} from "@/components/ui/button";
import {deleteRack} from "@/domain/rackControl";
import {toast} from "@/components/ui/use-toast";
import {useRouter} from "next/navigation"

interface DeleteRackDialogProps {
    rackId: number;
}

export function DeleteRackDialog({rackId}: DeleteRackDialogProps) {
    const router = useRouter();

    const handleDelete = async () => {
        try {
            await deleteRack(rackId)
            toast({
                title: "Selected rack has been deleted"
            });
            router.refresh(); // Refresh the data table to reflect changes
        } catch (error) {
            console.error(error);
            toast({
                title: "Unable to delete selected rack",
            });
        }
    };

    return (
        <>
            <DialogHeader>
                <DialogTitle>Are you absolutely sure?</DialogTitle>
                <DialogDescription>
                    This action cannot be undone. This will remove the rack {rackId} from the database.
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