"use client";

import {useEffect, useState} from "react";
import {zodResolver} from "@hookform/resolvers/zod";
import {useForm} from "react-hook-form";
import {z} from "zod";
import {Button} from "@/components/ui/button";
import {Form, FormControl, FormDescription, FormField, FormItem, FormLabel, FormMessage,} from "@/components/ui/form";
import {Input} from "@/components/ui/input";
import {toast} from "@/components/ui/use-toast";
import {readRfidTag} from "@/domain/rfidControl";
import {useRouter} from 'next/navigation';
import {createTraySchema} from "@/schema/custom";
import {Rack} from "@prisma/client";
import {Select, SelectContent, SelectItem, SelectTrigger, SelectValue} from "../ui/select";
import {createNewTray} from "@/domain/trayControl";

async function getData() {
    try {
        const response = await fetch('http://localhost:3000/api/rack');
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        const trays = await response.json();
        return trays;
    } catch (error) {
        console.error('There was a problem with the fetch operation:', error);
        return [];
    }
}

type CreateTrayFormValues = z.infer<typeof createTraySchema>;

export default function CreateTrayForm() {
    const router = useRouter();
    const form = useForm<CreateTrayFormValues>({
        resolver: zodResolver(createTraySchema),
        defaultValues: {
            epc: "",
            rackId: undefined,
        },
    });

    const [rackOptions, setRackOptions] = useState<Map<number, string>>(new Map());

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

    const [isScanDisabled, setScanDisabled] = useState(false);

    async function onSubmit(data: CreateTrayFormValues) {
        try {
            const result = await createNewTray(data.epc, data.rackId);
            if (result?.error) {
                toast({
                    title: result.error,
                    variant: "destructive",
                });
                return;
            }
            toast({
                title: "Tray has been created."
            });
        } catch (error) {
            toast({
                title: "Unable to create tray due to unknown error!",
                variant: "destructive",
            });
        }
    }

    async function handleScan() {
        setScanDisabled(true);
        const result = await readRfidTag();
        if (typeof result === "string") {
            form.setValue("epc", result);
        } else {
            toast({
                title: result.error,
                variant: "destructive",
            });
        }
        setScanDisabled(false);
    }

    return (
        <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
                <FormField
                    control={form.control}
                    name="epc"
                    render={({field}) => (
                        <FormItem>
                            <FormLabel>Tray EPC</FormLabel>
                            <FormControl>
                                <div className="flex space-x-4">
                                    <Input className="w-96" placeholder="Tray EPC" {...field} />
                                    <Button type="button" variant="secondary" onClick={handleScan}
                                            disabled={isScanDisabled}>Scan</Button>
                                </div>
                            </FormControl>
                            <FormDescription>
                                The EPC of the tray
                            </FormDescription>
                            <FormMessage/>
                        </FormItem>
                    )}
                />
                <FormField
                    control={form.control}
                    name="rackId"
                    render={({field}) => (
                        <FormItem>
                            <FormLabel>Rack</FormLabel>
                            <Select onValueChange={(value) => {
                                // Convert the string value back to a number for setting
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
                <Button type="submit">Create</Button>
            </form>
        </Form>
    );
}
