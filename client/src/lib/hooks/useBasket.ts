import { Item } from "../../app/models/basket";
import { useFetchBasketQuery, useClearBasketMutation } from "../../features/basket/basketApi";

export const useBasket = () => {
    const { data: basket } = useFetchBasketQuery();
    const [clearBasket] = useClearBasketMutation();

    const subTotal = basket?.items.reduce((sum: number, item: Item) => sum + item.quantity * item.price, 0) ?? 0;
    const deliveryFee = subTotal > 10000 ? 0 : 500;
    const total = subTotal + deliveryFee;

    return { basket, subTotal, deliveryFee, total, clearBasket }
}