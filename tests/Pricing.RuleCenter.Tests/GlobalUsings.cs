global using Pricing.RuleCenter.Core.Aggregates.Catalog;
global using Pricing.RuleCenter.Core.Aggregates.Charging;
global using Pricing.RuleCenter.Core.Aggregates.Quota;
global using Pricing.RuleCenter.Core.Aggregates.Rules;
global using Pricing.RuleCenter.Core.Interfaces.Catalog;
global using Pricing.RuleCenter.Core.Interfaces.Charging;
global using Pricing.RuleCenter.Core.Interfaces.Quota;
global using Pricing.RuleCenter.Core.Interfaces.Rules;
global using ChargeRequestLog = Pricing.RuleCenter.Core.Aggregates.Charging.ChargeRequest;
global using PricingApiCalculationDependencies = Pricing.RuleCenter.Application.Pricing.PricingAppCalculationDependencies;
global using PricingApiPersistenceRepositories = Pricing.RuleCenter.Application.Pricing.PricingAppPersistenceRepositories;
global using PricingApiService = Pricing.RuleCenter.Application.Pricing.PricingAppService;
global using RuleHeaderService = Pricing.RuleCenter.Application.Rules.RuleHeaderAppService;
global using RuleHeader = Pricing.RuleCenter.Core.Aggregates.Rules.RuleAggregate;
global using RulePublishService = Pricing.RuleCenter.Application.Rules.RulePublishAppService;


