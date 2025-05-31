"use client";

import {CardWrapper} from "@/components/auth/card-wrapper";
import {useForm} from "react-hook-form";
import {zodResolver} from "@hookform/resolvers/zod";
import * as z from "zod";
import {loginSchema} from "@/schema/custom";
import {Form, FormControl, FormField, FormItem, FormLabel, FormMessage} from "@/components/ui/form";
import {Input} from "@/components/ui/input";
import {Button} from "@/components/ui/button";
import {FormError} from "@/components/auth/form-error";
import {FormSuccess} from "@/components/auth/form-success";
import {useState, useTransition} from "react";
import {authenticateUser} from "@/domain/authControl";
import {useRouter} from 'next/navigation'


export const LoginForm = () => {
    const [error, setError] = useState<string | undefined>("");
    const [success, setSuccess] = useState<string | undefined>("");
    const [isPending, startTransition] = useTransition();

    const router = useRouter()


    const form = useForm<z.infer<typeof loginSchema>>({
        resolver: zodResolver(loginSchema),
        defaultValues: {
            email: "",
            password: "",
        }
    })

    const onSubmit = (values: z.infer<typeof loginSchema>) => {
        setError("");
        setSuccess("");
        startTransition(() => {
            authenticateUser(values)
                .then((data) => {
                    if (data) {
                        if (data.error) {
                            setError(data.error);
                        } else if (data.success) {
                            setSuccess(data.success);
                        }
                    } else {
                        setError("An unexpected error occurred.");
                    }
                })
                .catch((error) => {
                    setError("An unexpected error occurred.");
                });
        });
    };

    return (
        <CardWrapper headerLabel="Welcome back" backButtonLabel="Dont have an account?" backButtonHref="/auth/register">
            <Form {...form}>
                <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
                    <div className="space-y-4">
                        <FormField
                            control={form.control}
                            name="email"
                            render={({field}) => (
                                <FormItem>
                                    <FormLabel>Email</FormLabel>
                                    <FormControl>
                                        <Input {...field} disabled={isPending} placeholder="john.doe@example.com"
                                               type="email"/>
                                    </FormControl>
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
                                        <Input {...field} disabled={isPending} placeholder="******" type="password"/>
                                    </FormControl>
                                    <FormMessage/>
                                </FormItem>
                            )}
                        />
                    </div>
                    <FormError message={error}></FormError>
                    <FormSuccess message={success}></FormSuccess>
                    <Button disabled={isPending} type="submit" className="w-full"
                            onClick={() => router.push('/config/profile')}>
                        Login
                    </Button>
                </form>
            </Form>
        </CardWrapper>
    )
}