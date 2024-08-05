using System.Reactive.Disposables;
using Resto.Front.Api;
using Resto.Front.Api.Attributes;
using Resto.Front.Api.Data.Orders;
using Resto.Front.Api.UI;

namespace ExternalDataReader;

[PluginLicenseModuleId(21016318)]
public class Program : MarshalByRefObject, IFrontPlugin
{
    private readonly CompositeDisposable _disposable = new();
    
    public Program()
    {
        _disposable.Add(
            PluginContext.Operations.AddButtonToOrderEditScreen("External data", obj => ReadExternalData(obj.order, obj.vm, obj.os)));
        PluginContext.Operations.AddButtonToPaymentScreen("External data", false, true,
            obj => ReadExternalData(obj.order, obj.vm, obj.os));
    }
    
    private static void ReadExternalData(IOrder order, IViewManager viewManager, IOperationService operationService)
    {
        var actions = new Dictionary<int, (string Title, Action<IOrder, IViewManager, IOperationService> Action)>
        {
            [0] = ("Show all", ShowAllExternalData),
            [1] = ("Get by key", ShowExternalDataForSpecifyKey)
        };
        var chosenActionIndex = viewManager.ShowChooserPopup("Action", actions.OrderBy(x => x.Key).Select(x => x.Value.Title).ToList());
        
        if (actions.ContainsKey(chosenActionIndex))
            actions[chosenActionIndex].Action(order, viewManager, operationService);
    }
    
    private static void ShowAllExternalData(IOrder order, IViewManager viewManager, IOperationService operationService)
    {
        var allExternalData = operationService.GetOrderAllExternalData(order);
        viewManager.ShowOkPopup("External data", string.Join("\n", allExternalData));
    }
    
    private static void ShowExternalDataForSpecifyKey(IOrder order, IViewManager viewManager,
        IOperationService operationService)
    {
        var key = viewManager.ShowKeyboard("External data key");
        var ed = operationService.TryGetOrderExternalDataByKey(order, key);
        viewManager.ShowOkPopup($"External data for \"{key}\"", ed);
    }
    
    public void Dispose()
    {
        _disposable.Dispose();
    }
}