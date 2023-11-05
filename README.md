# FolderFilesManagment
It is for managing files in folders

For the first usage, I recomand creating a backup of the folder you want to use it on. Just for safety.

When starting the programm, you first have to select a folder.
Than you've 3 options:
1: Rename all files in all subfolders, according to their folder name. They names are sorted by the date of last write. (lastWriteTime)
2: Link all new files. You select a certain amount of days. Than all files not older than this will get linked or copied. The limit for copying is 50MB.
3: Rename all files, and thus who got renamed will get linked.
7: Prints the full blacklist file.
8: The user can change, what parameter of a file is used to sort files.
9: Changes the folder, what folder you want to manage files of.

Additionally, you can blacklist folders. If a folder is in a blacklist, than it and its subfolders will get ignored.
If you want to sort files different (for example creationTime), than you only have to edit one line inside the getFilesInFolder-method.
