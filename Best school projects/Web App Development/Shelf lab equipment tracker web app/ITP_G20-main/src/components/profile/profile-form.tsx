"use client"

import {zodResolver} from "@hookform/resolvers/zod"
import {useForm} from "react-hook-form"
import {z} from "zod"
import {Button} from "@/components/ui/button"
import {Form, FormControl, FormDescription, FormField, FormItem, FormLabel, FormMessage,} from "@/components/ui/form"
import {Input} from "@/components/ui/input"
import {toast} from "@/components/ui/use-toast"
import {updateProfileSchema} from "@/schema/custom"
import {updateProfile} from "@/domain/userControl"
import {useSession} from "next-auth/react"

type ProfileFormValues = z.infer<typeof updateProfileSchema>

export function ProfileForm() {
    const {data} = useSession();
    const email = data?.user?.email ?? ""

    const form = useForm<ProfileFormValues>({
        resolver: zodResolver(updateProfileSchema),
        defaultValues: {
            email: email,
            originalPassword: "",
            password: "",
            confirmPassword: "",
        },
    })

    async function onSubmit(data: ProfileFormValues) {
        const error = await updateProfile(data)
        console.log(error)
        if (error) {
            return toast({
                title: error.error,
            })
        } else {
            return toast({
                title: "Profile updated",
            })
        }
    }

    return (
        <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
                <FormField
                    control={form.control}
                    name="email"
                    render={({field}) => (
                        <FormItem>
                            <FormLabel>Email</FormLabel>
                            <FormControl>
                                <Input className="w-96" placeholder="Your email" {...field} />
                            </FormControl>
                            <FormDescription>
                                Your email is used to log in to your account.
                            </FormDescription>
                            <FormMessage/>
                        </FormItem>
                    )}
                />
                <FormField
                    control={form.control}
                    name="originalPassword"
                    render={({field}) => (
                        <FormItem>
                            <FormLabel>Current password</FormLabel>
                            <FormControl>
                                <Input className="w-96" placeholder="Your current password"
                                       type="password" {...field} />
                            </FormControl>
                            <FormDescription>
                                Please enter your current password.
                            </FormDescription>
                            <FormMessage/>
                        </FormItem>
                    )}
                />
                <FormField
                    control={form.control}
                    name="password"
                    render={({field}) => (
                        <FormItem>
                            <FormLabel>Password</FormLabel>
                            <FormControl>
                                <Input className="w-96" placeholder="Your password" type="password" {...field} />
                            </FormControl>
                            <FormDescription>
                                Your password must be at least 8 characters long.
                            </FormDescription>
                            <FormMessage/>
                        </FormItem>
                    )}
                />
                <FormField
                    control={form.control}
                    name="confirmPassword"
                    render={({field}) => (
                        <FormItem>
                            <FormLabel>Confirm password</FormLabel>
                            <FormControl>
                                <Input className="w-96" placeholder="Confirm your password"
                                       type="password" {...field} />
                            </FormControl>
                            <FormDescription>
                                Please confirm your password.
                            </FormDescription>
                            <FormMessage/>
                        </FormItem>
                    )}
                />
                <Button type="submit">Update account</Button>
            </form>
        </Form>
    )
}