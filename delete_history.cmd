echo ACHTUNG: Die gesamte History wird gelöscht. Drücke CTRL+C zum Abbrechen
pause
git checkout --orphan tmp-main
git add -A
git commit -m "Initial commit"
git branch -D main
git branch -m main
git push -f origin main
