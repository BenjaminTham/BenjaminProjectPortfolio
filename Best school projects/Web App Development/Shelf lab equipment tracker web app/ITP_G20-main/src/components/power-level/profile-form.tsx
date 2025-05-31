"use client"

import {zodResolver} from "@hookform/resolvers/zod"
import {useForm} from "react-hook-form"
import {z} from "zod"
import {Button} from "@/components/ui/button"
import {Form, FormControl, FormDescription, FormField, FormItem, FormLabel, FormMessage,} from "@/components/ui/form"
import {Input} from "@/components/ui/input"
import {toast} from "@/components/ui/use-toast"
import {powerLevelSchema} from "@/schema/custom"
import {getPowerLevelConfig, setPowerLevelConfig} from "@/domain/systemControl";
import {useEffect} from "react";

type PowerLevelFormValues = z.infer<typeof powerLevelSchema>

export function PowerLevelForm() {
    const form = useForm<PowerLevelFormValues>({
        resolver: zodResolver(powerLevelSchema),
    })

    useEffect(() => {
        getPowerLevelConfig().then((data) => {
            console.log(data)
            form.reset(data)
        })
    }, [form]);

    async function onSubmit(data: PowerLevelFormValues) {
        const error = await setPowerLevelConfig(data)
        console.log(error)
        if (error) {
            return toast({
                title: error.error,
            })
        } else {
            return toast({
                title: "Power level updated",
            })
        }
    }

    return (
        <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
                <FormField
                    control={form.control}
                    name="inventoryPower"
                    render={({field}) => (
                        <FormItem>
                            <FormLabel>Inventory power</FormLabel>
                            <FormControl>
                                <Input className="w-96" placeholder="Inventory power" {...field} />
                            </FormControl>
                            <FormDescription>
                                Set the power level for inventory.
                            </FormDescription>
                            <FormMessage/>
                        </FormItem>
                    )}
                />
                <FormField
                    control={form.control}
                    name="geofencingPower"
                    render={({field}) => (
                        <FormItem>
                            <FormLabel>Geofencing power</FormLabel>
                            <FormControl>
                                <Input className="w-96" placeholder="Geofencing power" {...field} />
                            </FormControl>
                            <FormDescription>
                                Set the power level for geofencing.
                            </FormDescription>
                            <FormMessage/>
                        </FormItem>
                    )}
                />
                <FormField
                    control={form.control}
                    name="readwritePower"
                    render={({field}) => (
                        <FormItem>
                            <FormLabel>Readwrite power</FormLabel>
                            <FormControl>
                                <Input className="w-96" placeholder="Readwrite power" {...field} />
                            </FormControl>
                            <FormDescription>
                                Set the power level for readwrite.
                            </FormDescription>
                            <FormMessage/>
                        </FormItem>
                    )}
                />
                <Button type="submit">Update power level</Button>
            </form>
        </Form>
    )
}