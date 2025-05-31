-- CreateTable
CREATE TABLE "Post" (
    "id" TEXT NOT NULL,

    CONSTRAINT "Post_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "User" (
    "id" TEXT NOT NULL,
    "email" TEXT NOT NULL,
    "password" TEXT NOT NULL,

    CONSTRAINT "User_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "Tray" (
    "id" TEXT NOT NULL,
    "tray_id" INTEGER NOT NULL,
    "rfid_id" INTEGER NOT NULL,
    "status" BOOLEAN NOT NULL DEFAULT false,
    "time_loaned" TIMESTAMP(3),
    "time_returned" TIMESTAMP(3),

    CONSTRAINT "Tray_pkey" PRIMARY KEY ("id")
);
