import {DialogClose, DialogDescription, DialogFooter, DialogHeader, DialogTitle} from "@/components/ui/dialog";
import {Button} from "@/components/ui/button";
import {toast} from "@/components/ui/use-toast";
import {useRouter} from "next/navigation"
import {Form, FormControl, FormDescription, FormField, FormItem, FormLabel, FormMessage} from '@/components/ui/form'
import {z} from "zod";
import {useForm} from "react-hook-form";
import {zodResolver} from "@hookform/resolvers/zod";
import {editTraySchema} from "@/schema/custom";
import {Select, SelectContent, SelectItem, SelectTrigger, SelectValue,} from "@/components/ui/select"
import {Rack} from "@prisma/client";
import {useEffect, useState} from "react";
import {reassignedTrayToRack} from "@/domain/trayControl";

async function getData() {
    try {
        const response = await fetch('http://localhost:3000/api/rack');
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        return await response.json();
    } catch (error) {
        console.error('There was a problem with the fetch operation:', error);
        return [];
    }
}

interface EditTrayDialogProps {
    trayId: number;
    rackId: number | null;
}

type EditTrayFormValues = z.infer<typeof editTraySchema>

export function EditTrayDialog({trayId, rackId}: EditTrayDialogProps) {
    const router = useRouter();
    const [rackOptions, setRackOptions] = useState<Map<number, string>>(new Map());
    const form = useForm<EditTrayFormValues>({
        resolver: zodResolver(editTraySchema),
        defaultValues: {
            id: trayId,
            rackId:  rackId || 0,
        },
    });

    useEffect(() => {
        const fetchData = async () => {
            const racks = await getData();
            const rackOptions = new Map<number, string>();
            rackOptions.set(0, "No rack");
            racks.forEach((rack: Rack) => {
                rackOptions.set(rack.id, `${rack.location} - ${rack.name}`);
            });
            setRackOptions(rackOptions);
        };
        fetchData();
    }, []);

    const handleEdit = async (data: EditTrayFormValues) => {
        try {
            if (data.rackId === undefined) {
                toast({
                    title: "Please select a rack",
                    variant: "destructive"
                });
                return;
            }
            console.log(data);
            const response = await reassignedTrayToRack(data);

            if (response?.error) {
                toast({
                    title: response.error,
                    variant: "destructive",
                });
                return;
            }
            toast({
                title: "Tray has been updated"
            });
            router.refresh(); // Refresh the data table to reflect changes
        } catch (error) {
            console.error(error);
            toast({
                title: "Unable to update tray",
            });
        }
    }

    return (
        <>
            <DialogHeader>
                <DialogTitle>Edit Tray Information</DialogTitle>
                <DialogDescription>
                    Make changes to tray {trayId} information. Please hold the tray near the RFID reader to overwrite the EPC.
                </DialogDescription>
            </DialogHeader>
            <Form {...form}>
                <form onSubmit={form.handleSubmit(handleEdit)} className="space-y-8 flex flex-col">
                    <FormField
                        control={form.control}
                        name="rackId"
                        render={({field}) => (
                            <FormItem>
                                <FormLabel>Rack</FormLabel>
                                <Select onValueChange={(value) => {
                                    const numericValue = Number(value);
                                    field.onChange(numericValue);
                                }} defaultValue={field.value?.toString()}>
                                    <FormControl>
                                        <SelectTrigger className="w-96">
                                            <SelectValue>
                                                {rackOptions.get(Number(field.value)) || 'Select a rack'}
                                            </SelectValue>
                                        </SelectTrigger>
                                    </FormControl>
                                    <SelectContent>
                                        {Array.from(rackOptions).map(([id, location]) => (
                                            <SelectItem key={id} value={id.toString()}>
                                                {location}
                                            </SelectItem>
                                        ))}
                                    </SelectContent>
                                </Select>
                                <FormDescription>
                                    The rack where the tray is located
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
                            <Button onClick={form.handleSubmit(handleEdit)} type="button">
                                Save
                            </Button>
                        </DialogClose>
                    </DialogFooter>
                </form>
            </Form>
        </>
    );
}