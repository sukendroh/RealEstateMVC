# RealEstateMVC – Build & Test Troubleshooting

If you're running into MSBuild errors like ...

```bash
"Could not copy file ... because it was not found"
```

... this usually means some build artifacts are out of sync or corrupted.

---

## Step 1: Clean Up Build Artifacts

Manually delete the `bin/` and `obj/` folders for both projects:

```bash
rm -r -f RealEstateMVC/bin RealEstateMVC/obj RealEstateMVC.Tests/bin RealEstateMVC.Tests/obj
```

## Step 2: Then run

```bash
dotnet clean
dotnet build RealEstateMVC.Tests
dotnet test RealEstateMVC.Tests
```