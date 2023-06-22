@echo off
cd C:\Users\72661341100.DFAD\source\repos\Backwards
echo fetching repo.
git fetch origin-push
echo pushing changes.
git push --force-with-lease origin-push
echo trying to pull.
git pull origin-push