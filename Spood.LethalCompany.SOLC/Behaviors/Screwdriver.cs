using Unity.Netcode;
using UnityEngine;

namespace Spood.LethalCompany.SOLC
{
    public class Screwdriver : PhysicsProp
    {
        private int mask = LayerMask.GetMask("Props");
        private StartOfRound playerManager;

        public override void Start()
        {
            base.Start();
            itemProperties.isConductiveMetal = true;
            playerManager = FindObjectOfType<StartOfRound>();
        }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            if (playerHeldBy == null || !IsOwner)
            {
                return;
            }

            var ray = new Ray(playerHeldBy.gameplayCamera.transform.position, playerHeldBy.gameplayCamera.transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 10f, mask))
            {
                var component = hit.transform.GetComponent<GrabbableObject>();
                if (component != null && component.itemProperties.isConductiveMetal)
                {
                    DismantleItemServerRpc(component.NetworkObject);
                }
            }
        }

        [ServerRpc]
        public void DismantleItemServerRpc(NetworkObjectReference networkObject)
        {
            Debug.Log("dismantle item server rpc called");
            NetworkObject itemToDismantle;
            if (networkObject.TryGet(out itemToDismantle))
            {
                Debug.Log("Dismantling item! " + itemToDismantle.name);
                itemToDismantle.Despawn(true);
                Item scrapItem = Plugin.bundle.LoadAsset<Item>("Assets/Spood.LethalCompany.SOLC/ScrapMetal/ScrapMetal.asset");

                var newPosition = itemToDismantle.transform.position + new Vector3(0, 0.3f, 0);

                var scrap = Instantiate(scrapItem.spawnPrefab, newPosition, itemToDismantle.transform.rotation, playerManager.propsContainer);
                scrap.GetComponent<GrabbableObject>().fallTime = 0f;
                scrap.GetComponent<NetworkObject>().Spawn();
            }
        }
    }
}
