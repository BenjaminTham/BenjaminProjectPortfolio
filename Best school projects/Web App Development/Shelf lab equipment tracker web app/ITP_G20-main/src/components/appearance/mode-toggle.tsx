"use client"

import {useTheme} from "next-themes"
import {Select, SelectContent, SelectItem, SelectTrigger, SelectValue} from "@/components/ui/select"
import {useEffect, useState} from "react"

export function ModeToggle() {
    const {theme, setTheme} = useTheme()
    const [mounted, setMounted] = useState(false)

    useEffect(() => {
        setMounted(true)
    }, [])

    if (!mounted) {
        return null
    }

    return (
        <Select defaultValue={theme} onValueChange={(value) => setTheme(value)}>
            <SelectTrigger className="w-[180px]">
                <SelectValue placeholder="Theme"/>
            </SelectTrigger>
            <SelectContent>
                <SelectItem value="light">Light</SelectItem>
                <SelectItem value="dark">Dark</SelectItem>
                <SelectItem value="system">System</SelectItem>
            </SelectContent>
        </Select>
    )
}