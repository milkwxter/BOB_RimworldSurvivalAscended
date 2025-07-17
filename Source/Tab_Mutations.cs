using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace BOB_ArkMod
{
    public static class SpecimenMutationTabDrawer
    {
        public class MutationNode
        {
            public string label;
            public string iconName;
            public List<MutationNode> children;
            public bool unlocked;
            public Vector2 position;

            public MutationNode(string label, string iconName, List<MutationNode> children, bool unlocked, Vector2 position)
            {
                this.label = label;
                this.iconName = iconName;
                this.children = children;
                this.unlocked = unlocked;
                this.position = position;
            }
        }

        private static readonly Texture2D dnaChain = ContentFinder<Texture2D>.Get("UI/dna", true);

        public static List<MutationNode> mutationList = new List<MutationNode>
        {
            new MutationNode("Claws", "m_mutation_claws", new List<MutationNode>(), false, new Vector2(200f, 50f)),
            new MutationNode("HeadFeathers", "m_mutation_head_feathers", new List<MutationNode>(), false, new Vector2(50f, 50f)),
            new MutationNode("VenomFangs", "m_mutation_venom_fangs", new List<MutationNode>(), false, new Vector2(350f, 50f)),
            new MutationNode("ToughHide", "m_mutation_tough_hide", new List<MutationNode>(), false, new Vector2(200f, 150f)),
            new MutationNode("FearRoar", "m_mutation_fear_roar", new List<MutationNode>(), false, new Vector2(50f, 150f)),
        };

        private struct ConnectionRenderData
        {
            public Rect drawRect;
            public float angle;
        }

        private static List<ConnectionRenderData> cachedConnections = null;

        public static void BuildMutationConnections()
        {
            if (cachedConnections != null) return;
            cachedConnections = new List<ConnectionRenderData>();

            MutationNode claws = mutationList.Find(m => m.label == "Claws");
            MutationNode headFeathers = mutationList.Find(m => m.label == "HeadFeathers");
            MutationNode venomFangs = mutationList.Find(m => m.label == "VenomFangs");
            MutationNode toughHide = mutationList.Find(m => m.label == "ToughHide");
            MutationNode fearRoar = mutationList.Find(m => m.label == "FearRoar");

            // build children lists
            claws.children.Add(headFeathers);
            claws.children.Add(venomFangs);
            claws.children.Add(toughHide);
            claws.children.Add(fearRoar);

            foreach (var parent in mutationList)
            {
                foreach (var child in parent.children)
                {
                    Vector2 start = parent.position + new Vector2(32f, 32f);
                    Vector2 end = child.position + new Vector2(32f, 32f);
                    Vector2 direction = (end - start).normalized;
                    float distance = Vector2.Distance(start, end);
                    float stepSize = 64f;
                    float textureHeight = 64f;
                    int steps = Mathf.FloorToInt(distance / stepSize);

                    for (int i = 0; i <= steps; i++)
                    {
                        Vector2 drawPos = start + direction * (i * stepSize);
                        Rect drawRect = new Rect(0f, 0f, stepSize, textureHeight);
                        drawRect.center = drawPos;
                        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                        cachedConnections.Add(new ConnectionRenderData
                        {
                            drawRect = drawRect,
                            angle = angle
                        });
                    }
                }
            }
        }

        public static void DrawMutationTab(Rect rect, HediffComp_SpecimenImplant comp)
        {
            GUI.BeginGroup(rect);

            // draw the background
            GUI.DrawTexture(new Rect(0f, 0f, rect.width, rect.height), ContentFinder<Texture2D>.Get("UI/Widgets/DesButBG"));

            // draw the connections
            foreach (var conn in cachedConnections)
            {
                Matrix4x4 oldMatrix = GUI.matrix;
                GUIUtility.RotateAroundPivot(conn.angle, conn.drawRect.center);
                GUI.DrawTexture(conn.drawRect, dnaChain);
                GUI.matrix = oldMatrix;
            }

            // draw the nodes
            foreach (MutationNode node in mutationList)
            {
                DrawNode(node, comp);
            }

            GUI.EndGroup();
        }

        public static void DrawNode(MutationNode node, HediffComp_SpecimenImplant comp)
        {
            // draw the node background then the icon
            Rect nodeRect = new Rect(node.position.x, node.position.y, 64f, 64f);
            GUI.DrawTexture(nodeRect, ContentFinder<Texture2D>.Get("UI/Widgets/DesButBG"));
            Texture2D icon = ContentFinder<Texture2D>.Get("UI/Mutations/" + node.iconName, true);
            GUI.DrawTexture(nodeRect, icon);

            // get the state of the mutation
            bool isUnlocked = comp.HasThisMutation(node.label);

            // show if its unlocked or not
            Color overlay = isUnlocked ? new Color(0f, 1f, 0f, 0.15f) : new Color(1f, 0f, 0f, 0.15f);
            Widgets.DrawBoxSolid(nodeRect, overlay);

            TooltipHandler.TipRegion(nodeRect, node.label);

            // allow user to unlock the mutation
            if (!isUnlocked && Widgets.ButtonInvisible(nodeRect))
            {
                comp.AddMutation(node.label);
                Messages.Message($"Unlocked mutation: {node.label}", MessageTypeDefOf.PositiveEvent);
            }
        }
    }
}
