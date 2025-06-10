// Global namespace aliases para desacoplar el código del nombre del proyecto
// Permite cambiar el nombre del proyecto modificando solo este archivo
//
// NOTA: Este archivo solo define aliases para el propio proyecto Contracts
// Los otros proyectos deben definir sus propios aliases en sus GlobalUsings.cs

// Aliases para namespaces de Contracts (seguros de usar)
global using ContractsValidation = PimFlow.Contracts.Validation;
global using ContractsConfiguration = PimFlow.Contracts.Configuration;
global using ContractsEnums = PimFlow.Contracts.Enums;
global using ContractsCommon = PimFlow.Contracts.Common;

// Comentarios para documentar los aliases que otros proyectos pueden usar:
//
// En Domain/GlobalUsings.cs:
// global using Domain = PimFlow.Domain;
// global using DomainEntities = PimFlow.Domain.Entities;
// global using DomainValueObjects = PimFlow.Domain.ValueObjects;
//
// En Shared/GlobalUsings.cs:
// global using Shared = PimFlow.Shared;
// global using SharedViewModels = PimFlow.Shared.ViewModels;
// global using SharedDTOs = PimFlow.Shared.DTOs;
//
// En Server/GlobalUsings.cs:
// global using Server = PimFlow.Server;
// global using ServerServices = PimFlow.Server.Services;
// global using ServerControllers = PimFlow.Server.Controllers;
//
// En Client/GlobalUsings.cs:
// global using Client = PimFlow.Client;
// global using ClientServices = PimFlow.Client.Services;
// global using ClientComponents = PimFlow.Client.Components;
