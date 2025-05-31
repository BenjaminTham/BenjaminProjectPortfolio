import { MainNav } from "@/components/nav/main-nav"
import { MobileNav } from "@/components/nav/mobile-nav"
import { SystemStatusPopover } from "@/components/nav/system-status-popover"
import { currentUser } from "@/lib/auth";
import { LoginLogoutButton } from "@/components/nav/login-logout-buttons";
import IdleTimer from "@/components/idle-timer/idle-timer";

export async function SiteHeader() {
    const user = await currentUser();

    return (
        <header
            className="sticky top-0 z-50 w-full border-b border-border/40 bg-background/95 backdrop-blur supports-[backdrop-filter]:bg-background/60">
            <div className="container flex h-14 max-w-screen-2xl items-center">
                <IdleTimer user={user} />
                <MainNav />
                <MobileNav />
                <div className="flex flex-1 items-end md:justify-end">
                    <nav className="flex items-center ml-auto md:ml-0 space-x-4">
                        <SystemStatusPopover user={user} />
                        <LoginLogoutButton user={user} />
                    </nav>
                </div>
            </div>
        </header>
    )
}