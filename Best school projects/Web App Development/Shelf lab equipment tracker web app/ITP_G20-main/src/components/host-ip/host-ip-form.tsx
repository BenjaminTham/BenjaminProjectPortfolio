"use client"

import {zodResolver} from "@hookform/resolvers/zod"
import {useForm} from "react-hook-form"
import {z} from "zod"
import {Button} from "@/components/ui/button"
import {Form, FormControl, FormDescription, FormField, FormItem, FormLabel, FormMessage,} from "@/components/ui/form"
import {Input} from "@/components/ui/input"
import {toast} from "@/components/ui/use-toast"
import {editHostIpSchema} from "@/schema/custom"
import {useEffect, useState} from "react";
import {getHostIpConfig, setHostIpConfig} from "@/domain/systemControl";

type EditIpHostFormValues = z.infer<typeof editHostIpSchema>

export function HostIpForm() {
    const [ip, setIp] = useState<string>("")

    const form = useForm<EditIpHostFormValues>({
        resolver: zodResolver(editHostIpSchema),
        defaultValues: {
            ip: ip,
        },
    })

    useEffect(() => {
        const fetchData = async () => {
            const ipData = await getHostIpConfig();
            setIp(ipData as string);
            form.setValue('ip', ipData as string); // Update the form field directly
        };

        fetchData();
    }, [form]);

    async function onSubmit(data: EditIpHostFormValues) {
        const error = await setHostIpConfig(data)
        if (error) {
            toast({
                variant: "destructive",
                title: error.error,
            })
        } else {
            toast({
                title: "IP Address updated",
            })
            form.setValue('ip', data.ip);
        }
    }

    return (
        <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-8 flex space-x-4">
                <FormField
                    control={form.control}
                    name="ip"
                    render={({ field }) => (
                        <FormItem>
                            <FormLabel>IP Address</FormLabel>
                            <FormControl>
                                <Input placeholder="IP Address" {...field} />
                            </FormControl>
                            <FormDescription>
                                The IP address of the host machine.
                            </FormDescription>
                            <FormMessage />
                        </FormItem>
                    )}
                />
                <Button type="submit" className="">Edit IP</Button>
            </form>
        </Form>
    )
}