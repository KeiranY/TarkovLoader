using System.Collections.Generic;
using System;
using TarkovLoader;
using EFT.InventoryLogic;
using HarmonyLib;

namespace ItemValue
{
    public class ItemValue : BaseMod
    {
        public override Dictionary<string, string> DefaultOptions => null;

        public override void Init(Dictionary<string, string> options, BaseLoader loader)
        {
            base.Init(options, loader);
            var harmony = new HarmonyLib.Harmony("com.kcy.ItemValuePatch");
            harmony.PatchAll();
        }

        public static void AddItemValue<T>(ref T __instance, string id, ItemTemplate template) where T : Item
        {
            // Remove item if it has no value
            // if (Math.Round(__instance.Value()) == 0) return;

            // Make a copy of the existing attributes list, this is needed for inherited types of Item that use a global attributes list (ammo)
            var atts = new List<GClass1758>();
            atts.AddRange(__instance.Attributes);
            __instance.Attributes = atts;

            GClass1758 attr = new GClass1758(EItemAttributeId.MoneySum)
            {
                Name = "RUB ₽",
                StringValue = new Func<string>(__instance.ValueStr),
                DisplayType = new Func<EItemAttributeDisplayType>(() => EItemAttributeDisplayType.Compact)
            };
            __instance.Attributes.Add(attr);
        }
    }
    public static class ValueExtension
    {
        public static double Value(this Item item)
        {
            double price = item.Template.CreditsPrice;

            // Container
            if (item is GClass1662 container)
            {
                foreach (var slot in container.Slots)
                {
                    foreach (var i in slot.Items)
                    {
                        price += i.Value();
                    }
                }
            }

            if (item is GClass1700 mag)
            {
                foreach (var i in mag.Cartridges.Items)
                {
                    price += i.Value();
                }

            }

            if (item is Weapon wep)
            {
                foreach (Slot s in wep.Chambers)
                {
                    foreach (Item i in s.Items)
                    {
                        price += i.Value();
                    }
                }
            }

            var medKit = item.GetItemComponent<MedKitComponent>();
            if (medKit != null)
            {
                price *= medKit.HpResource / medKit.MaxHpResource;
            }

            var repair = item.GetItemComponent<RepairableComponent>();
            if (repair != null)
            {
                price *= repair.Durability / repair.MaxDurability;
            }

            var dogtag = item.GetItemComponent<DogtagComponent>();
            if (dogtag != null)
            {
                price *= dogtag.Level;
            }

            price *= item.StackObjectsCount;

            return price;
        }
        public static string ValueStr(this Item item)
        {
            return Math.Round(item.Value()).ToString();
        }
    }

    [HarmonyPatch]
    class ValuePatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Item), MethodType.Constructor, new Type[] { typeof(string), typeof(ItemTemplate) })]
        static void PostfixItem(ref Item __instance, string id, ItemTemplate template) => ItemValue.AddItemValue(ref __instance, id, template);
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(GClass1746), MethodType.Constructor, new Type[] { typeof(string), typeof(AmmoTemplate) })]
        static void PostfixAmmo(ref GClass1746 __instance, string id, AmmoTemplate template) => ItemValue.AddItemValue(ref __instance, id, template);

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GClass1748), MethodType.Constructor, new Type[] { typeof(string), typeof(GClass1648) })]
        static void PostfixGrenade(ref GClass1748 __instance, string id, GClass1648 template) => ItemValue.AddItemValue(ref __instance, id, template);

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GClass1712), MethodType.Constructor, new Type[] { typeof(string), typeof(GClass1615) })]
        static void PostfixConainer(ref GClass1748 __instance, string id, GClass1615 template) => ItemValue.AddItemValue(ref __instance, id, template);
    }
}
