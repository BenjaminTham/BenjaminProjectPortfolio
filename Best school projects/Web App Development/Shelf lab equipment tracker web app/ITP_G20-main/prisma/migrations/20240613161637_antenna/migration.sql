-- AlterEnum
ALTER TYPE "Function" ADD VALUE 'NONE';

-- DropForeignKey
ALTER TABLE "Antenna" DROP CONSTRAINT "Antenna_readerId_fkey";

-- AlterTable
ALTER TABLE "Antenna" ALTER COLUMN "readerId" DROP NOT NULL;

-- AddForeignKey
ALTER TABLE "Antenna" ADD CONSTRAINT "Antenna_readerId_fkey" FOREIGN KEY ("readerId") REFERENCES "Reader"("id") ON DELETE SET NULL ON UPDATE CASCADE;
