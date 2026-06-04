# TODO - CalculadorLogistico

- [x] Review existing implementation files
- [x] Add custom exceptions: `UnknownZoneException`, `UnknownRouteException`
- [x] Extend `CalcularTarifaEnvio` signature with `out string log`
- [x] Validate origin/destination zones exist in baseTariffs keys; throw `UnknownZoneException`
- [x] Direct route A→B: compute and set log
- [x] Inverse route: if A→B missing but B→A exists, compute inverse with 10% surcharge and set log
- [x] Intermediate route: find B where A→B and B→C exist, sum both and set log
- [x] Round final costs to 2 decimals
- [x] Update unit tests for new method signature and exception type
- [ ] Run `dotnet test` and confirm passing

