# Packager

This is a script that helps in streamlining the process of packaging.

## How to Get Started

1. In the Project Window > Create > Framework > `Packager`.
2. Select the created object and click on > `Set Defaults` in the `Package Info` tab.
3. Enter `Package Name`, `Package Version`, and `Package Description` in the `Package Info` tab.
4. Drag the files you want to include into the `Assets` field in the `Package Info` tab.
5. You can customize the `Output Directory` in the `Export Settings` tab.
6. You can set the Unity's export options flags in `Options` in the `Export Settings` tab.
7. You can add exclusions by adding strings in `Exclude Starts With` and `Exclude Ends With` in the `Export Settings` tab. (This only works when using the `Experimental Export` button)
8. Use the `Export` button if you want to use Unity's default dependency finder.
9. Use the `Experimental Export` button if you want the packager to find the required dependencies and exclude files that matched in `Exclude Starts With` and `Exclude Ends With` fields.

## Bugs


## To-Do's

1. Add regular expressions filtering when excluding files.
2. Code cleanup.
3. Changelogs.