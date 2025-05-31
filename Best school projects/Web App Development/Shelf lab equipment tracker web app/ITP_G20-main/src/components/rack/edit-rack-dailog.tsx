import {DialogClose, DialogDescription, DialogFooter, DialogHeader, DialogTitle} from "@/components/ui/dialog";
import {Button} from "@/components/ui/button";
import {updateRack} from "@/domain/rackControl";
import {toast} from "@/components/ui/use-toast";
import {useRouter} from "next/navigation"
import {Form, FormControl, FormDescription, FormField, FormItem, FormLabel, FormMessage} from '@/components/ui/form'
import {z} from "zod";
import {updateRackSchema} from "@/schema/custom";
import {useForm} from "react-hook-form";
import {zodResolver} from "@hookform/resolvers/zod";
import {Input} from "@/components/ui/input";


interface EditRackDialogProps {
    rackId: number;
    location: string;
    name: string;
}

type EditRackFormValues = z.infer<typeof updateRackSchema>

export function EditRackDialog({rackId, location, name}: EditRackDialogProps) {
    const router = useRouter();
    const form = useForm<EditRackFormValues>({
        resolver: zodResolver(updateRackSchema),
        defaultValues: {
            rackId: rackId,
            location: location,
            name: name
        },
    });

    const handleEdit = async (data: EditRackFormValues) => {
        try {
            await updateRack(data);
            toast({
                title: "Rack has been updated"
            });
            router.refresh(); // Refresh the data table to reflect changes
        } catch (error) {
            console.error(error);
            toast({
                title: "Unable to update rack",
            });
        }
    }

    return (
        <>
            <DialogHeader>
                <DialogTitle>Edit Rack Information</DialogTitle>
                <DialogDescription>
                    Make changes to rack {rackId} information.
                </DialogDescription>
            </DialogHeader>
            <Form {...form}>
                <form onSubmit={form.handleSubmit(handleEdit)}>
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

        </>
    );
}