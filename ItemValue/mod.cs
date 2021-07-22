using System.Collections.Generic;
using System;
using TarkovLoader;
using EFT.InventoryLogic;
using HarmonyLib;

using Ammo = GClass1746;
using Grenade = GClass1748;
using GrenadeTemplate = GClass1648;
using SecureContainer = GClass1712;
using SecureContainerTemplate = GClass1615;
using Container = GClass1662;
using Magazine = GClass1700;
using ItemAttribute = GClass1758;

namespace ItemValue
{
    public class ItemValue : BaseMod
    {
        public override Dictionary<string, string> DefaultOptions => null;

        public override void Init(Dictionary<string, string> options, BaseLoader loader)
        {
            base.Init(options, loader);
            var harmony = new Harmony("com.kcy.ItemValuePatch");
            harmony.PatchAll();
        }

        public static void AddItemValue<T>(ref T __instance, string id, ItemTemplate template) where T : Item
        {
            // Remove item if it has no value
            // if (Math.Round(__instance.Value()) == 0) return;

            // Make a copy of the existing attributes list, this is needed for inherited types of Item that use a global attributes list (ammo)
            var atts = new List<ItemAttribute>();
            atts.AddRange(__instance.Attributes);
            __instance.Attributes = atts;

            ItemAttribute attr = new ItemAttribute(EItemAttributeId.MoneySum)
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
            if (item is Container container)
            {
                foreach (var slot in container.Slots)
                {
                    foreach (var i in slot.Items)
                    {
                        price += i.Value();
                    }
                }
                foreach (var c in container.Containers)
                {
                    foreach (var i in c.Items)
                    {
                        price += i.Value();
                    }
                }
            }

            if (item is Magazine mag)
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
        [HarmonyPatch(typeof(Ammo), MethodType.Constructor, new Type[] { typeof(string), typeof(AmmoTemplate) })]
        static void PostfixAmmo(ref Ammo __instance, string id, AmmoTemplate template) => ItemValue.AddItemValue(ref __instance, id, template);

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Grenade), MethodType.Constructor, new Type[] { typeof(string), typeof(GrenadeTemplate) })]
        static void PostfixGrenade(ref Grenade __instance, string id, GrenadeTemplate template) => ItemValue.AddItemValue(ref __instance, id, template);

        [HarmonyPostfix]
        [HarmonyPatch(typeof(SecureContainer), MethodType.Constructor, new Type[] { typeof(string), typeof(SecureContainerTemplate) })]
        static void PostfixConainer(ref SecureContainer __instance, string id, SecureContainerTemplate template) => ItemValue.AddItemValue(ref __instance, id, template);
    }
}
