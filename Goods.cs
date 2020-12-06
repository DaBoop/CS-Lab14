using System;
using System.Collections.Generic;
using System.Text;

namespace Lab14
{
    [Serializable]
    abstract public class Goods
    {
        public int mass { get; set; } // почему не сказали сразу, а ток в 6 лабе               .............................
        public int amount;

        public string name { get; set; }
        public int price { get; set; }

        protected Goods() { } // зачем.

        public Goods(string arg_name, int arg_amount, int arg_price, int arg_mass) => (name, amount, price, mass) = (arg_name, arg_amount, arg_price, arg_mass);

    }
        [Serializable]
    abstract class Product : Goods
    {
        public int energyValue { get; set; }

        //public int mass { get;  set; } // и ещё разок почему.... .... 

        protected Product() { }
        public Product(string arg_name, int arg_amount, int arg_price, int arg_mass, int arg_energyValue) => (name, amount, price, mass, energyValue) = (arg_name, arg_amount, arg_price, arg_mass, arg_energyValue);

        public override string ToString()
        {
            return $"{amount} of {name} products, {price}$. Mass: {mass}. Energy value: {energyValue} kcal";
        }


    }
    [Serializable]
    abstract class Confectionery : Product
    {
        public int sugarValue { get; set; }

        protected Confectionery() { }
        public Confectionery(string arg_name, int arg_amount, int arg_price, int arg_mass, int arg_energyValue, int arg_sugarValue) => (name, amount, price, mass, energyValue, sugarValue) = (arg_name, arg_amount, arg_price, arg_mass, arg_energyValue, sugarValue);

        public override string ToString()
        {
            return $"{amount} of {name} confectionery products, {price}$ each. Mass: {mass}. Energy value: {energyValue} kcal. Sugar value: {sugarValue} g.";
        }
    }
    [Serializable]
    sealed class Cake : Confectionery
    {
        public int area { get; set; }
        public int height { get; set; }
        public Cake(string arg_name, int arg_amount, int arg_price, int arg_mass, int arg_energyValue, int arg_sugarValue, int arg_area, int arg_height) => (name, amount, price, mass, energyValue, sugarValue, area, height) = (arg_name, arg_amount, arg_price, arg_mass, arg_energyValue, arg_sugarValue, arg_area, arg_height);

        public Cake() { }
        public override string ToString()
        {
            return $"{amount} of {name} cakes, {price}$ each. Mass: {mass}. Energy value: {energyValue} kcal. Sugar value: {sugarValue} g.";
        }
    }
    [Serializable]
    sealed class Sweets : Confectionery
    {
        public int amountPerKg { get; set; }
        public Sweets(string arg_name, int arg_amount, int arg_price, int arg_mass, int arg_energyValue, int arg_sugarValue, int arg_amountPerKg) => (name, amount, price, mass, energyValue, sugarValue, amountPerKg) = (arg_name, arg_amount, arg_price, arg_mass, arg_energyValue, arg_sugarValue, arg_amountPerKg);

        public override string ToString()
        {
            return $"{amount} of {name} sweets, {price}$ per kg. Mass: {mass}. Energy value: {energyValue} kcal. Sugar value: {sugarValue} g. Average amount/kg: {amountPerKg}";
        }
    }
}
