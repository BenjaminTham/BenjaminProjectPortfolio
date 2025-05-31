import type {Metadata} from "next";
import {Inter} from "next/font/google";
import "./globals.css";
import {ThemeProvider} from "@/components/shared/theme-provider";
import {SiteHeader} from "@/components/nav/site-header";
import {Toaster} from "@/components/ui/toaster";
import {SessionProvider} from "next-auth/react";
import {auth} from "@/auth";
import {GeofencingAlert} from "@/components/geofencing/geofencing-alert";

const inter = Inter({subsets: ["latin"]});

export const metadata: Metadata = {
    title: "SIT-RFID by ITP_G20",
    description: "RFiD management system proudly build by ITP G20 2024"
};

export default async function RootLayout({children,}: Readonly<{ children: React.ReactNode; }>) {
    const session = await auth();
    return (
        <SessionProvider session={session}>
            <html lang="en">
            <body className={inter.className}>
            <ThemeProvider
                attribute="class"
                defaultTheme="system"
                enableSystem
                disableTransitionOnChange
            >
                <SiteHeader/>
                {children}
                <GeofencingAlert/>
                <Toaster/>
            </ThemeProvider>
            </body>
            </html>
        </SessionProvider>
    );
}
