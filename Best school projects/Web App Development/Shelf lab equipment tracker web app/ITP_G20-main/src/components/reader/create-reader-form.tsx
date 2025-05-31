//src/components/reader/host-ip-form.tsx

"use client"

import {zodResolver} from "@hookform/resolvers/zod"
import {useForm} from "react-hook-form"
import {z} from "zod"
import {Button} from "@/components/ui/button"
import {Form, FormControl, FormDescription, FormField, FormItem, FormLabel, FormMessage,} from "@/components/ui/form"
import {Input} from "@/components/ui/input"
import {toast} from "@/components/ui/use-toast"
import {createReaderFormSchema} from "@/schema/custom"
import {addNewReader} from "@/domain/readerControl"

type CreateReaderFormValues = z.infer<typeof createReaderFormSchema>

export function CreateReaderForm() {
    const form = useForm<CreateReaderFormValues>({
        resolver: zodResolver(createReaderFormSchema),
        defaultValues: {
            ip: "",
        },
    })

    async function onSubmit(data: CreateReaderFormValues) {
        const error = await addNewReader(data)
        if (error) {
            toast({
                variant: "destructive",
                title: error.error,
            })
        } else {
            toast({
                title: "Reader created",
            })
        }
        form.reset()
    }

    return (
        <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-8 flex space-x-4">
                <FormField
                    control={form.control}
                    name="ip"
                    render={({field}) => (
                        <FormItem>
                            <FormLabel>IP Address</FormLabel>
                            <FormControl>
                                <Input placeholder="IP Address" {...field} />
                            </FormControl>
                            <FormDescription>
                                The IP address of the reader.
                            </FormDescription>
                            <FormMessage/>
                        </FormItem>
                    )}
                />
                <Button type="submit" className="">Create Reader</Button>
            </form>
        </Form>
    )
}