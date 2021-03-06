﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MyCaffe.basecode;
using MyCaffe.common;

namespace MyCaffe.param
{
    /// <summary>
    /// Specifies the base parameter for all layers.
    /// </summary>
    public class LayerParameter : BaseParameter, ICloneable, IComparable, IBinaryPersist  
    {
        // The layer name.
        string m_strName;
        // The layer type.
        LayerType m_type;
        // The name of each bottom blob.
        List<string> m_rgstrBottom = new List<string>();
        // The name of each top blob.
        List<string> m_rgstrTop = new List<string>();

        // The train/test phase for computation.
        Phase m_phase;

        // The amout of weight to assign each top blob in the objective.
        // Each layer assigns a default value, usually of either 0 or 1,
        // to each top blob.
        List<double> m_rgLossWeight = new List<double>();

        // Specifies training parameters (multipliers on global learning constants,
        // and the name and other settings used for weight sharing).
        List<ParamSpec> m_rgParams = new List<ParamSpec>();

        /// <summary>
        /// The blobs containing the numeric parameters of the layer.
        /// </summary>
        List<BlobProto> m_rgBlobs = new List<BlobProto>();

        /// <summary>
        /// Specifies whether to backpropagate to each bottom.  If specified,
        /// Caffe will automatically infer whether each input needs backpropagation
        /// to compute parameter gradients.  If set to true for some inputs,
        /// backpropagation to those inputs is forced; if set to false for some inputs,
        /// backpropagation to those inputs is skipped.
        /// </summary>
        List<bool> m_rgbPropagateDown = new List<bool>();

        /// <summary>
        /// Rules controlling whether and when a layer is included in the network,
        /// based on the current NetState.  You may specify a non-zero number of rules
        /// to include OR exclude, but not both.  If no include or exclude rules are
        /// specified, the layer is always included.  If the current NetState meets
        /// ANY (i.e,, one or more) of the specified rules, the layer is
        /// included/excluded.
        /// </summary>
        List<NetStateRule> m_rgInclude = new List<NetStateRule>();
        List<NetStateRule> m_rgExclude = new List<NetStateRule>();
        Dictionary<Phase, int> m_rgMaxBottomCount = new Dictionary<Phase, int>();

        int m_nSolverCount = 1;
        int m_nSolverRank = 0;
        List<string> m_rgstrExpectedTop = new List<string>();
        List<string> m_rgstrExpectedBottom = new List<string>();

        /// <summary>
        /// Specifies the layer type.
        /// </summary>
        public enum LayerType
        {
            /// <summary>
            /// Initializes a parameter for the AbsValLayer.
            /// </summary>
            ABSVAL,
            /// <summary>
            /// Initializes a parameter for the AccuracyLayer.
            /// </summary>
            ACCURACY,
            /// <summary>
            /// Initializes a parameter for the ArgMaxLayer.
            /// </summary>
            ARGMAX,
            /// <summary>
            /// Initializes a parameter for the BiasLayer.
            /// </summary>
            BIAS,
            /// <summary>
            /// Initializes a parameter for the BatchNormLayer.
            /// </summary>
            BATCHNORM,
            /// <summary>
            /// Initializes a parameter for the BatchReindexLayer.
            /// </summary>
            BATCHREINDEX,
            /// <summary>
            /// Initializes a parameter for the BNLLLayer.
            /// </summary>
            BNLL,
            /// <summary>
            /// Initializes a parameter for the ConcatLayer.
            /// </summary>
            CONCAT,
            /// <summary>
            /// Initializes a parameter for the ContrastiveLossLayer.
            /// </summary>
            CONTRASTIVE_LOSS,
            /// <summary>
            /// Initializes a parameter for the ConvolutionLayer.
            /// </summary>
            CONVOLUTION,
            /// <summary>
            /// Initializes a parameter for the CropLayer.
            /// </summary>
            CROP,
            /// <summary>
            /// Initializes a parameter for the DeconvolutionLayer.
            /// </summary>
            DECONVOLUTION,
            /// <summary>
            /// Initializes a parameter for the DataLayer.
            /// </summary>
            DATA,
            /// <summary>
            /// Initializes a parameter for the DropoutLayer.
            /// </summary>
            DROPOUT,
            /// <summary>
            /// Initializes a parameter for the DummyDataLayer.
            /// </summary>
            DUMMYDATA,
            /// <summary>
            /// Initializes a parameter for the EltwiseLayer.
            /// </summary>
            ELTWISE,
            /// <summary>
            /// Initializes a parameter for the ELULayer.
            /// </summary>
            ELU,
            /// <summary>
            /// Initializes a parameter for the EmbedLayer.
            /// </summary>
            EMBED,
            /// <summary>
            /// Initializes a parameter for the EuclideanLossLayer.
            /// </summary>
            EUCLIDEAN_LOSS,
            /// <summary>
            /// Initializes a parameter for the ExpLayer.
            /// </summary>
            EXP,
            /// <summary>
            /// Initializes a parameter for the FilterLayer.
            /// </summary>
            FILTER,
            /// <summary>
            /// Initializes a parameter for the FlattenLayer.
            /// </summary>
            FLATTEN,
            /// <summary>
            /// Initializes a parameter for the GradScaleLayer (used for gradient reversal)
            /// </summary>
            GRADIENTSCALER,
            /// <summary>
            /// Initializes a parameter for the GRNLayer (global response normalization L2)
            /// </summary>
            GRN,
            /// <summary>
            /// Initializes a parameter for the HingeLossLayer.
            /// </summary>
            HINGE_LOSS,
            /// <summary>
            /// Initializes a parameter for the Im2ColLayer.
            /// </summary>
            IM2COL,
            /// <summary>
            /// Initializes a parameter for the InfogainLossLayer.
            /// </summary>
            INFOGAIN_LOSS,
            /// <summary>
            /// Initializes a parameter for the InnerProductLayer.
            /// </summary>
            INNERPRODUCT,
            /// <summary>
            /// Initializes a parameter for the LabelMappingLayer.
            /// </summary>
            LABELMAPPING,
            /// <summary>
            /// Initializes a parameter for the LogLayer.
            /// </summary>
            LOG,
            /// <summary>
            /// Initializes a parameter for the LossLayer.
            /// </summary>
            LOSS,
            /// <summary>
            /// Initializes a parameter for the LRNLayer.
            /// </summary>
            LRN,
            /// <summary>
            /// Initializes a parameter for the MemoryDataLayer.
            /// </summary>
            MEMORYDATA,
            /// <summary>
            /// Initializes a parameter for the MultinomialLogisticLossLayer.
            /// </summary>
            MULTINOMIALLOGISTIC_LOSS,
            /// <summary>
            /// Initializes a parameter for the MVNLayer.
            /// </summary>
            MVN,
            /// <summary>
            /// Initializes a parameter for the PoolingLayer.
            /// </summary>
            POOLING,
            /// <summary>
            /// Initializes a parameter for the PowerLayer.
            /// </summary>
            POWER,
            /// <summary>
            /// Initializes a parameter for the PReLULayer.
            /// </summary>
            PRELU,
            /// <summary>
            /// Initializes a parameter for the ReductionLayer.
            /// </summary>
            REDUCTION,
            /// <summary>
            /// Initializes a parameter for the ReLULayer.
            /// </summary>
            RELU,
            /// <summary>
            /// Initializes a parameter for the ReshapeLayer.
            /// </summary>
            RESHAPE,
            /// <summary>
            /// Initializes a parameter for the ScaleLayer.
            /// </summary>
            SCALE,
            /// <summary>
            /// Initializes a parameter for the SigmoidLayer.
            /// </summary>
            SIGMOID,
            /// <summary>
            /// Initializes a parameter for the SigmoidCrossEntropyLossLayer.
            /// </summary>
            SIGMOIDCROSSENTROPY_LOSS,
            /// <summary>
            /// Initializes a parameter for the SoftmaxLayer.
            /// </summary>
            SOFTMAX,
            /// <summary>
            /// Initializes a parameter for the SoftmaxLossLayer.
            /// </summary>
            SOFTMAXWITH_LOSS,
            /// <summary>
            /// Initializes a parameter for the SPPLayer.
            /// </summary>
            SPP,
            /// <summary>
            /// Initializes a parameter for the SilenceLayer.
            /// </summary>
            SILENCE,
            /// <summary>
            /// Initializes a parameter for the SliceLayer.
            /// </summary>
            SLICE,
            /// <summary>
            /// Initializes a parameter for the SplitLayer.
            /// </summary>
            SPLIT,
            /// <summary>
            /// Initializes a parameter for the SwishLayer
            /// </summary>
            SWISH,
            /// <summary>
            /// Initializes a parameter for the TanhLayer.
            /// </summary>
            TANH,
            /// <summary>
            /// Initializes a parameter for the ThresholdLayer.
            /// </summary>
            THRESHOLD,
            /// <summary>
            /// Initializes a parameter for the TileLayer.
            /// </summary>
            TILE,
            /// <summary>
            /// Initializes a parameter for the DataTransformer.
            /// </summary>
            TRANSFORM,
            /// <summary>
            /// Initializes a parameter for the LSTMSimpleLayer.
            /// </summary>
            LSTM_SIMPLE,
            /// <summary>
            /// Initializes a parameter for the RecurrentLayer.
            /// </summary>
            RECURRENT,
            /// <summary>
            /// Initializes a parameter for the RNNLayer.
            /// </summary>
            RNN,
            /// <summary>
            /// Initializes a parameter for the LSTMLayer.
            /// </summary>
            LSTM,
            /// <summary>
            /// Initializes a parameter for the LSTMUnitLayer.
            /// </summary>
            LSTM_UNIT,
            /// <summary>
            /// Initializes a parameter for the InputLayer.
            /// </summary>
            INPUT,
            /// <summary>
            /// Initializes a parameter for the BatchDataLayer.
            /// </summary>
            BATCHDATA,
            /// <summary>
            /// Initializes a parameter for the ReinforcementLossLayer.
            /// </summary>
            REINFORCEMENT_LOSS,
            /// <summary>
            /// Initializes a parameter for the UnpoolingLayer1 which uses a CPU based implementation (slower).
            /// </summary>
            UNPOOLING1,
            /// <summary>
            /// Initializes a parameter for the UnpoolingLayer2 which uses a GPU based implementation (faster).
            /// </summary>
            UNPOOLING2,
            /// <summary>
            /// Initializes a parameter for the NormalizationLayer.
            /// </summary>
            NORMALIZATION,
            /// <summary>
            /// Initializes a parameter for the TripletLossSimpleLayer.
            /// </summary>
            TRIPLET_LOSS_SIMPLE,
            /// <summary>
            /// Initializes a parameter for the TripletLossLayer.
            /// </summary>
            TRIPLET_LOSS,
            /// <summary>
            /// Initializes a parameter for the TripletSelectLayer.
            /// </summary>
            TRIPLET_SELECT,
            /// <summary>
            /// Initializes a parameter for the TripletDataLayer.
            /// </summary>
            TRIPLET_DATA,
            /// <summary>
            /// Initializes a parameter for the KNNLayer.
            /// </summary>
            KNN,
            /// <summary>
            /// Initializes a parameter for the DebugLayer.
            /// </summary>
            DEBUG,
            /// <summary>
            /// Initializes a parameter for the BinaryHashLayer.
            /// </summary>
            BINARYHASH,
            _MAX
        }

        // Layer type-specific parameters
        //
        // Note: certain layers may have more than one computation engine
        // for their implementation. These layers include an Engine type and
        // engine parameter for selecting the implementation.
        // The default for the engine is set by the ENGINE switch at compile-time.
        Dictionary<LayerType, LayerParameterBase> m_rgLayerParameters = new Dictionary<LayerType, LayerParameterBase>();

        /** @copydoc BaseParameter */
        public LayerParameter() : base()
        {
            for (int i = 0; i < (int)LayerType._MAX; i++)
            {
                m_rgLayerParameters.Add((LayerType)i, null);
            }
        }

        /// <summary>
        /// The LayerParameter constructor.
        /// </summary>
        /// <param name="lt">Assignes this LayerType to the layer.</param>
        /// <param name="strName">Assigns this name to the layer.</param>
        public LayerParameter(LayerType lt, string strName = null)
            : base()
        {
            m_type = lt;
            m_strName = strName;

            if (m_strName == null)
                m_strName = lt.ToString();

            for (int i = 0; i < (int)LayerType._MAX; i++)
            {
                m_rgLayerParameters.Add((LayerType)i, null);
            }

            setupParams(lt);
        }

        /// <summary>
        /// The LayerParameter constructor.
        /// </summary>
        /// <param name="p">Used to initialize the new LayerParameter.</param>
        public LayerParameter(LayerParameter p)
            : base()
        {
            m_type = p.m_type;
            m_strName = p.m_strName;
            m_rgstrBottom = p.m_rgstrBottom;
            m_rgstrTop = p.m_rgstrTop;
            m_phase = p.m_phase;
            m_rgLossWeight = p.m_rgLossWeight;
            m_rgParams = p.m_rgParams;
            m_rgBlobs = p.m_rgBlobs;
            m_rgbPropagateDown = p.m_rgbPropagateDown;
            m_rgInclude = p.m_rgInclude;
            m_rgExclude = p.m_rgExclude;
            m_rgLayerParameters = p.m_rgLayerParameters;
            m_nSolverCount = p.m_nSolverCount;
            m_nSolverRank = p.m_nSolverRank;
        }

        /// <summary>
        /// Returns the number of ParamSpec parameters used by the layer.
        /// </summary>
        /// <returns>The ParamSpec count is returned.</returns>
        public int GetParameterCount()
        {
            int nOffset = 0;

            switch (m_type)
            {
                case LayerType.CONVOLUTION:
                case LayerType.DECONVOLUTION:
                    if (convolution_param != null && !convolution_param.bias_term && m_rgParams.Count > 1)
                        nOffset = -1;
                    break;

                case LayerType.INNERPRODUCT:
                    if (inner_product_param != null && !inner_product_param.bias_term && m_rgParams.Count > 1)
                        nOffset = -1;
                    break;
            }

            return m_rgParams.Count + nOffset;
        }

        /// <summary>
        /// Copies the defaults from another LayerParameter.
        /// </summary>
        /// <param name="p">Specifies the LayerParameter to copy.</param>
        public void CopyDefaults(LayerParameter p)
        {
            if (p == null)
                return;

            if (p.type != m_type)
                throw new ArgumentOutOfRangeException();

            m_rgInclude = p.include;
            m_rgExclude = p.exclude;
            m_rgParams = p.parameters;

            switch (m_type)
            {
                case LayerType.DATA:
                case LayerType.TRIPLET_DATA:       
                    data_param = (DataParameter)p.data_param.Clone();
                    transform_param = (TransformationParameter)p.transform_param.Clone();
                    break;

                case LayerType.BATCHDATA:
                    batch_data_param = (BatchDataParameter)p.batch_data_param.Clone();
                    transform_param = (TransformationParameter)p.transform_param.Clone();
                    break;
            }
        }

        /// <summary>
        /// Determines whether or not this LayerParameter meets a given Phase.
        /// </summary>
        /// <param name="phase">Specifies the Phase.</param>
        /// <returns>Returns <i>true</i> if this LayerParameter meets the Phase, <i>false</i> otherwise.</returns>
        public bool MeetsPhase(Phase phase)
        {
            if (phase == Phase.NONE)
                return true;

            foreach (NetStateRule r in m_rgExclude)
            {
                if (r.phase == phase)
                    return false;
            }

            foreach (NetStateRule r in m_rgInclude)
            {
                if (r.phase == phase)
                    return true;
            }

            if (m_rgInclude.Count == 0)
                return true;

            if (m_rgExclude.Count > 0)
                return true;

            return false;
        }

        /// <summary>
        /// Save this parameter to a binary writer.
        /// </summary>
        /// <param name="bw">Specifies the binary writer to use.</param>
        public void Save(BinaryWriter bw)
        {
            bw.Write((int)m_type);
            bw.Write(m_strName);
            Utility.Save<string>(bw, m_rgstrBottom);
            Utility.Save<string>(bw, m_rgstrTop);
            Utility.Save<double>(bw, m_rgLossWeight);
            Utility.Save<ParamSpec>(bw, m_rgParams);
            Utility.Save<BlobProto>(bw, m_rgBlobs);
            Utility.Save<bool>(bw, m_rgbPropagateDown);
            Utility.Save<NetStateRule>(bw, m_rgInclude);
            Utility.Save<NetStateRule>(bw, m_rgExclude);

            int nCount = 0;

            foreach (LayerParameterBase p in m_rgLayerParameters.Values)
            {
                if (p != null)
                    nCount++;
            }

            bw.Write(nCount);

            foreach (KeyValuePair<LayerType, LayerParameterBase> kv in m_rgLayerParameters)
            {
                if (kv.Value != null)
                {
                    bw.Write((int)kv.Key);

                    IBinaryPersist bp = kv.Value as IBinaryPersist;
                    bp.Save(bw);
                }
            }
        }

        /// <summary>
        /// Load the parameter from a binary reader.
        /// </summary>
        /// <param name="br">Specifies the binary reader.</param>
        /// <param name="bNewInstance">When <i>true</i> a new instance is created (the default), otherwise the existing instance is loaded from the binary reader.</param>
        /// <returns>Returns an instance of the parameter.</returns>
        public object Load(BinaryReader br, bool bNewInstance)
        {
            LayerType lt = (LayerType)br.ReadInt32();
            string strName = br.ReadString();

            LayerParameter p = this;
            
            if (bNewInstance)
                p = new LayerParameter(lt, strName);

            p.m_rgstrBottom = Utility.Load<string>(br);
            p.m_rgstrTop = Utility.Load<string>(br);
            p.m_rgLossWeight = Utility.Load<double>(br);
            p.m_rgParams = Utility.Load<ParamSpec>(br);
            p.m_rgBlobs = Utility.Load<BlobProto>(br);
            p.m_rgbPropagateDown = Utility.Load<bool>(br);
            p.m_rgInclude = Utility.Load<NetStateRule>(br);
            p.m_rgExclude = Utility.Load<NetStateRule>(br);

            int nCount = br.ReadInt32();

            for (int i = 0; i < nCount; i++)
            {
                lt = (LayerType)br.ReadInt32();
                IBinaryPersist bp = m_rgLayerParameters[lt] as IBinaryPersist;
                bp.Load(br, false);
            }

            return p;
        }

        private void setupParams(LayerType lt)
        {
            switch (lt)
            {
                case LayerType.ABSVAL:
                    expected_bottom.Add("input");
                    expected_top.Add("abs");
                    break;

                case LayerType.ACCURACY:
                    expected_bottom.Add("input");
                    expected_bottom.Add("label");
                    expected_top.Add("accuracy");
                    m_rgLayerParameters[lt] = new AccuracyParameter();
                    break;

                case LayerType.ARGMAX:
                    expected_bottom.Add("input");
                    expected_top.Add("max");
                    m_rgLayerParameters[lt] = new ArgMaxParameter();
                    break;

                case LayerType.BATCHDATA:
                    expected_bottom.Add("imgidx");
                    expected_top.Add("data");
                    expected_top.Add("label");
                    m_rgLayerParameters[LayerType.TRANSFORM] = new TransformationParameter();
                    m_rgLayerParameters[lt] = new BatchDataParameter();
                    break;

                case LayerType.BATCHNORM:
                    expected_bottom.Add("input");
                    expected_top.Add("norm");
                    m_rgLayerParameters[lt] = new BatchNormParameter();
                    break;

                case LayerType.BATCHREINDEX:
                    expected_bottom.Add("input");
                    expected_bottom.Add("idx");
                    expected_top.Add("data");
                    break;

                case LayerType.BIAS:
                    expected_bottom.Add("input");
                    expected_bottom.Add("bias");
                    expected_top.Add("bias");
                    m_rgLayerParameters[lt] = new BiasParameter();
                    break;

                case LayerType.BINARYHASH:
                    expected_bottom.Add("input1");
                    expected_bottom.Add("input2");
                    expected_bottom.Add("input3");
                    expected_bottom.Add("label");
                    expected_top.Add("binhash");
                    m_rgLayerParameters[lt] = new BinaryHashParameter();
                    break;

                case LayerType.BNLL:
                    expected_bottom.Add("input");
                    expected_top.Add("bnll");
                    break;

                case LayerType.CONCAT:
                    expected_bottom.Add("x_1");
                    expected_bottom.Add("x_2");
                    expected_top.Add("concat");
                    m_rgLayerParameters[lt] = new ConcatParameter(); 
                    break;

                case LayerType.CONTRASTIVE_LOSS:
                    expected_bottom.Add("a");
                    expected_bottom.Add("b");
                    expected_bottom.Add("s");
                    expected_top.Add("loss");
                    m_rgLayerParameters[LayerType.LOSS] = new LossParameter();
                    m_rgLayerParameters[lt] = new ContrastiveLossParameter();
                    break;

                case LayerType.CONVOLUTION:
                case LayerType.IM2COL:
                    expected_bottom.Add("input");
                    expected_top.Add("output");
                    m_rgLayerParameters[LayerType.CONVOLUTION] = new ConvolutionParameter();
                    break;

                case LayerType.DECONVOLUTION:
                    expected_bottom.Add("score");
                    expected_top.Add("upscore");
                    m_rgLayerParameters[LayerType.CONVOLUTION] = new ConvolutionParameter();
                    break;

                case LayerType.CROP:
                    expected_bottom.Add("upscore");
                    expected_bottom.Add("data");
                    expected_top.Add("score");
                    m_rgLayerParameters[LayerType.CROP] = new CropParameter();
                    break;

                case LayerType.DATA:
                case LayerType.TRIPLET_DATA:
                case LayerType.MEMORYDATA:
                    expected_top.Add("data");
                    expected_top.Add("label");
                    m_rgLayerParameters[LayerType.TRANSFORM] = new TransformationParameter();
                    if (lt == LayerType.MEMORYDATA)
                        m_rgLayerParameters[LayerType.MEMORYDATA] = new MemoryDataParameter();
                    else
                        m_rgLayerParameters[LayerType.DATA] = new DataParameter();
                    break;

                case LayerType.DEBUG:
                    expected_bottom.Add("input");
                    expected_bottom.Add("label");
                    expected_top.Add("output");
                    m_rgLayerParameters[lt] = new DebugParameter();
                    break;

                case LayerType.DROPOUT:
                    expected_bottom.Add("input");
                    expected_top.Add("dropout");
                    m_rgLayerParameters[lt] = new DropoutParameter();
                    break;

                case LayerType.DUMMYDATA:
                    expected_top.Add("data");
                    expected_top.Add("label");
                    m_rgLayerParameters[LayerType.TRANSFORM] = new TransformationParameter();
                    m_rgLayerParameters[lt] = new DummyDataParameter();
                    break;

                case LayerType.ELTWISE:
                    expected_bottom.Add("x_1");
                    expected_bottom.Add("x_2");
                    expected_top.Add("eltwise");
                    m_rgLayerParameters[lt] = new EltwiseParameter();
                    break;

                case LayerType.ELU:
                    expected_bottom.Add("input");
                    expected_top.Add("elu");
                    m_rgLayerParameters[lt] = new EluParameter();
                    break;

                case LayerType.EMBED:
                    expected_bottom.Add("input");
                    expected_top.Add("embed");
                    m_rgLayerParameters[lt] = new EmbedParameter();
                    break;

                case LayerType.EUCLIDEAN_LOSS:
                    expected_bottom.Add("pred");
                    expected_bottom.Add("trgt");
                    expected_top.Add("loss");
                    m_rgLayerParameters[LayerType.LOSS] = new LossParameter();
                    break;

                case LayerType.EXP:
                    expected_bottom.Add("input");
                    expected_top.Add("exp");
                    m_rgLayerParameters[lt] = new ExpParameter();
                    break;

                case LayerType.FILTER:
                    expected_bottom.Add("x_1");
                    expected_bottom.Add("x_2");
                    expected_top.Add("y_1");
                    expected_top.Add("y_2");
                    break;

                case LayerType.FLATTEN:
                    expected_bottom.Add("x_1");
                    expected_bottom.Add("x_2");
                    expected_top.Add("flatten");
                    m_rgLayerParameters[lt] = new FlattenParameter();
                    break;

                case LayerType.GRADIENTSCALER:
                    expected_bottom.Add("input");
                    expected_top.Add("identity");
                    m_rgLayerParameters[lt] = new GradientScaleParameter();
                    break;

                case LayerType.GRN:
                    expected_bottom.Add("input");
                    expected_top.Add("grn");
                    m_rgLayerParameters[lt] = new FlattenParameter();
                    break;

                case LayerType.HINGE_LOSS:
                    expected_bottom.Add("pred");
                    expected_bottom.Add("label");
                    expected_top.Add("loss");
                    m_rgLayerParameters[LayerType.LOSS] = new LossParameter();
                    m_rgLayerParameters[lt] = new HingeLossParameter();
                    break;

                case LayerType.INFOGAIN_LOSS:
                    expected_bottom.Add("pred");
                    expected_bottom.Add("label");
                    expected_bottom.Add("H");
                    expected_top.Add("loss");
                    m_rgLayerParameters[LayerType.LOSS] = new LossParameter();
                    m_rgLayerParameters[lt] = new InfogainLossParameter();
                    break;

                case LayerType.INNERPRODUCT:
                    expected_bottom.Add("input");
                    expected_top.Add("ip");
                    m_rgLayerParameters[lt] = new InnerProductParameter();
                    break;

                case LayerType.LABELMAPPING:
                    expected_bottom.Add("input");
                    expected_top.Add("output");
                    m_rgLayerParameters[lt] = new LabelMappingParameter();
                    break;

                case LayerType.KNN:
                    expected_bottom.Add("input");
                    expected_bottom.Add("label");
                    expected_top.Add("classes");
                    m_rgMaxBottomCount.Add(Phase.RUN, 1);
                    m_rgLayerParameters[lt] = new KnnParameter();
                    break;

                case LayerType.LOG:
                    expected_bottom.Add("input");
                    expected_top.Add("log");
                    m_rgLayerParameters[lt] = new LogParameter();
                    break;

                case LayerType.LRN:
                    expected_bottom.Add("input");
                    expected_top.Add("lrn");
                    m_rgLayerParameters[lt] = new LRNParameter();
                    break;

                case LayerType.MULTINOMIALLOGISTIC_LOSS:
                    expected_bottom.Add("pred");
                    expected_bottom.Add("label");
                    expected_top.Add("loss");
                    m_rgLayerParameters[LayerType.LOSS] = new LossParameter();
                    break;

                case LayerType.MVN:
                    expected_bottom.Add("input");
                    expected_top.Add("mvn");
                    m_rgLayerParameters[lt] = new MVNParameter();
                    break;

                case LayerType.NORMALIZATION:
                    expected_bottom.Add("input");
                    expected_top.Add("norm");
                    m_rgLayerParameters[lt] = new NormalizationParameter();
                    break;

                case LayerType.POOLING:
                    expected_bottom.Add("input");
                    expected_top.Add("pool");
                    m_rgLayerParameters[LayerType.POOLING] = new PoolingParameter();
                    break;

                case LayerType.UNPOOLING1:
                case LayerType.UNPOOLING2:
                    expected_bottom.Add("pool");
                    expected_bottom.Add("mask");
                    expected_top.Add("unpool");
                    m_rgLayerParameters[LayerType.POOLING] = new PoolingParameter();
                    break;

                case LayerType.POWER:
                    expected_bottom.Add("input");
                    expected_top.Add("power");
                    m_rgLayerParameters[lt] = new PowerParameter();
                    break;

                case LayerType.PRELU:
                    expected_bottom.Add("input");
                    expected_top.Add("prelu");
                    m_rgLayerParameters[lt] = new PReLUParameter();
                    break;

                case LayerType.REDUCTION:
                    expected_bottom.Add("input");
                    expected_top.Add("reduction");
                    m_rgLayerParameters[lt] = new ReductionParameter();
                    break;

                case LayerType.REINFORCEMENT_LOSS:
                    expected_bottom.Add("pred");
                    expected_bottom.Add("label");
                    expected_top.Add("loss");
                    m_rgLayerParameters[lt] = new ReinforcementLossParameter();
                    break;

                case LayerType.RELU:
                    expected_bottom.Add("input");
                    expected_top.Add("relu");
                    m_rgLayerParameters[lt] = new ReLUParameter();
                    break;

                case LayerType.RESHAPE:
                    expected_bottom.Add("input");
                    expected_top.Add("reshape");
                    m_rgLayerParameters[lt] = new ReshapeParameter();
                    break;

                case LayerType.SCALE:
                    expected_bottom.Add("input");
                    expected_top.Add("scale");
                    m_rgLayerParameters[lt] = new ScaleParameter();
                    break;

                case LayerType.SIGMOID:
                    expected_bottom.Add("input");
                    expected_top.Add("sigmoid");
                    m_rgLayerParameters[lt] = new SigmoidParameter();
                    break;

                case LayerType.SIGMOIDCROSSENTROPY_LOSS:
                    expected_bottom.Add("scores");
                    expected_bottom.Add("trgt");
                    expected_top.Add("loss");
                    m_rgLayerParameters[LayerType.LOSS] = new LossParameter(LossParameter.NormalizationMode.BATCH_SIZE);
                    m_rgLayerParameters[LayerType.SIGMOID] = new SigmoidParameter();
                    break;

                case LayerType.SILENCE:
                    break;

                case LayerType.SLICE:
                    expected_bottom.Add("input");
                    expected_top.Add("slice1");
                    expected_top.Add("slice2");
                    m_rgLayerParameters[lt] = new SliceParameter();
                    break;

                case LayerType.SOFTMAX:
                    expected_bottom.Add("input");
                    expected_top.Add("softmax");
                    m_rgLayerParameters[lt] = new SoftmaxParameter();
                    break;

                case LayerType.SOFTMAXWITH_LOSS:
                    expected_bottom.Add("pred");
                    expected_bottom.Add("label");
                    expected_top.Add("loss");
                    m_rgLayerParameters[LayerType.SOFTMAX] = new SoftmaxParameter();
                    m_rgLayerParameters[LayerType.LOSS] = new LossParameter();
                    break;

                case LayerType.SPLIT:
                    expected_bottom.Add("input");
                    expected_top.Add("split1");
                    expected_top.Add("split2");
                    break;

                case LayerType.SPP:
                    expected_bottom.Add("input");
                    expected_top.Add("spp");
                    m_rgLayerParameters[lt] = new SPPParameter();
                    break;

                case LayerType.SWISH:
                    expected_bottom.Add("input");
                    expected_top.Add("swish");
                    m_rgLayerParameters[lt] = new SwishParameter();
                    break;

                case LayerType.TANH:
                    expected_bottom.Add("input");
                    expected_top.Add("tanh");
                    m_rgLayerParameters[lt] = new TanhParameter();
                    break;

                case LayerType.THRESHOLD:
                    expected_bottom.Add("input");
                    expected_top.Add("thresh");
                    m_rgLayerParameters[lt] = new ThresholdParameter();
                    break;

                case LayerType.TILE:
                    expected_bottom.Add("input");
                    expected_top.Add("tile");
                    m_rgLayerParameters[lt] = new TileParameter();
                    break;

                case LayerType.TRIPLET_LOSS_SIMPLE:
                    expected_bottom.Add("input");
                    expected_bottom.Add("label");
                    expected_top.Add("loss");
                    m_rgLayerParameters[lt] = new TripletLossSimpleParameter();
                    m_rgLayerParameters[LayerType.LOSS] = new LossParameter();
                    break;

                case LayerType.TRIPLET_LOSS:
                    expected_bottom.Add("anchor");
                    expected_bottom.Add("pos");
                    expected_bottom.Add("neg");
                    expected_bottom.Add("label");
                    expected_top.Add("loss");
                    m_rgLayerParameters[lt] = new TripletLossParameter();
                    m_rgLayerParameters[LayerType.LOSS] = new LossParameter();
                    break;

                case LayerType.TRIPLET_SELECT:
                    expected_bottom.Add("input");
                    expected_top.Add("anchor");
                    expected_top.Add("pos");
                    expected_top.Add("neg");
                    break;

                case LayerType.LSTM_SIMPLE:
                    expected_bottom.Add("input");
                    expected_top.Add("lstm");
                    m_rgLayerParameters[LayerType.LSTM_SIMPLE] = new LSTMSimpleParameter();
                    break;

                case LayerType.RNN:
                    expected_bottom.Add("time");
                    expected_bottom.Add("seq");
                    expected_bottom.Add("stat");
                    expected_top.Add("rnn");
                    m_rgLayerParameters[LayerType.RECURRENT] = new RecurrentParameter();
                    break;

                case LayerType.LSTM:
                    expected_bottom.Add("time");
                    expected_bottom.Add("seq");
                    expected_bottom.Add("stat");
                    expected_top.Add("lstm");
                    m_rgLayerParameters[LayerType.RECURRENT] = new RecurrentParameter();
                    break;

                case LayerType.INPUT:
                    m_rgLayerParameters[LayerType.INPUT] = new InputParameter();
                    break;
            }
        } 

        /// <summary>
        /// Specifies the name of this LayerParameter.
        /// </summary>
        public string name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }

        /// <summary>
        /// Specifies the type of this LayerParameter.
        /// </summary>
        public LayerType type
        {
            get { return m_type; }
            set { m_type = value; }
        }

        /// <summary>
        /// Specifies the active bottom connections (in the bottom, out the top).
        /// </summary>
        public List<string> bottom
        {
            get { return m_rgstrBottom; }
            set { m_rgstrBottom = value; }
        }

        /// <summary>
        /// Specifies the active top connections (in the bottom, out the top)
        /// </summary>
        public List<string> top
        {
            get { return m_rgstrTop; }
            set { m_rgstrTop = value; }
        }

        /// <summary>
        /// Specifies the Phase for which this LayerParameter is run.
        /// </summary>
        public Phase phase
        {
            get { return m_phase; }
            set { m_phase = value; }
        }

        /// <summary>
        /// Specifies the loss weight.
        /// </summary>
        public List<double> loss_weight
        {
            get { return m_rgLossWeight; }
            set { m_rgLossWeight = value; }
        }

        /// <summary>
        /// Specifies the ParamSpec parameters of the LayerParameter.
        /// </summary>
        public List<ParamSpec> parameters
        {
            get { return m_rgParams; }
            set { m_rgParams = value; }
        }

        /// <summary>
        /// Specifies the blobs of the LayerParameter.
        /// </summary>
        public List<BlobProto> blobs
        {
            get { return m_rgBlobs; }
            set { m_rgBlobs = value; }
        }

        /// <summary>
        /// Specifies whether or not the LayerParameter (or protions of) should be backpropagated.
        /// </summary>
        public List<bool> propagate_down
        {
            get { return m_rgbPropagateDown; }
            set { m_rgbPropagateDown = value; }
        }

        /// <summary>
        /// Specifies the NetStateRule's for which this LayerParameter should be included.
        /// </summary>
        public List<NetStateRule> include
        {
            get { return m_rgInclude; }
            set { m_rgInclude = value; }
        }

        /// <summary>
        /// Specifies the NetStateRule's for which this LayerParameter should be excluded.
        /// </summary>
        public List<NetStateRule> exclude
        {
            get { return m_rgExclude; }
            set { m_rgExclude = value; }
        }

        /// <summary>
        /// Returns the parameter set when initialized with LayerType.TRANSFORM
        /// </summary>
        public TransformationParameter transform_param
        {
            get { return (TransformationParameter)m_rgLayerParameters[LayerType.TRANSFORM]; }
            set { m_rgLayerParameters[LayerType.TRANSFORM] = value; }
        }

        /// <summary>
        /// Returns the parameter set when initialized with LayerType.LOSS
        /// </summary>
        public LossParameter loss_param
        {
            get { return (LossParameter)m_rgLayerParameters[LayerType.LOSS]; }
            set { m_rgLayerParameters[LayerType.LOSS] = value; }
        }

        /// <summary>
        /// Returns the parameter set when initialized with LayerType.ACCURACY
        /// </summary>
        public AccuracyParameter accuracy_param
        {
            get { return (AccuracyParameter)m_rgLayerParameters[LayerType.ACCURACY]; }
            set { m_rgLayerParameters[LayerType.ACCURACY] = value; }
        }

        /// <summary>
        /// Returns the parameter set when initialized with LayerType.ARGMAX
        /// </summary>
        public ArgMaxParameter argmax_param
        {
            get { return (ArgMaxParameter)m_rgLayerParameters[LayerType.ARGMAX]; }
            set { m_rgLayerParameters[LayerType.ARGMAX] = value; }
        }

        /// <summary>
        /// Returns the parameter set when initialized with LayerType.BATCHDATA
        /// </summary>
        public BatchDataParameter batch_data_param
        {
            get { return (BatchDataParameter)m_rgLayerParameters[LayerType.BATCHDATA]; }
            set { m_rgLayerParameters[LayerType.BATCHDATA] = value; }
        }

        /// <summary>
        /// Returns the parameter set when initialized with LayerType.BATCHNORM
        /// </summary>
        public BatchNormParameter batch_norm_param
        {
            get { return (BatchNormParameter)m_rgLayerParameters[LayerType.BATCHNORM]; }
            set { m_rgLayerParameters[LayerType.BATCHNORM] = value; }
        }

        /// <summary>
        /// Returns the parameter set when initialized with LayerType.BIAS
        /// </summary>
        public BiasParameter bias_param
        {
            get { return (BiasParameter)m_rgLayerParameters[LayerType.BIAS]; }
            set { m_rgLayerParameters[LayerType.BIAS] = value; }
        }

        /// <summary>
        /// Returns the parameter set when initialized with LayerType.BINARYHASH
        /// </summary>
        public BinaryHashParameter binary_hash_param
        {
            get { return (BinaryHashParameter)m_rgLayerParameters[LayerType.BINARYHASH]; }
            set { m_rgLayerParameters[LayerType.BINARYHASH] = value; }
        }

        /// <summary>
        /// Returns the parameter set when initialized with LayerType.CONCAT
        /// </summary>
        public ConcatParameter concat_param
        {
            get { return (ConcatParameter)m_rgLayerParameters[LayerType.CONCAT]; }
            set { m_rgLayerParameters[LayerType.CONCAT] = value; }
        }

        /// <summary>
        /// Returns the parameter set when initialized with LayerType.CONTRASTIVE_LOSS
        /// </summary>
        public ContrastiveLossParameter contrastive_loss_param
        {
            get { return (ContrastiveLossParameter)m_rgLayerParameters[LayerType.CONTRASTIVE_LOSS]; }
            set { m_rgLayerParameters[LayerType.CONTRASTIVE_LOSS] = value; }
        }

        /// <summary>
        /// Returns the parameter set when initialized with LayerType.CONVOLUTION
        /// </summary>
        public ConvolutionParameter convolution_param
        {
            get { return (ConvolutionParameter)m_rgLayerParameters[LayerType.CONVOLUTION]; }
            set { m_rgLayerParameters[LayerType.CONVOLUTION] = value; }
        }

        /// <summary>
        /// Returns the parameter set when initialized with LayerType.CROP
        /// </summary>
        public CropParameter crop_param
        {
            get { return (CropParameter)m_rgLayerParameters[LayerType.CROP]; }
            set { m_rgLayerParameters[LayerType.CROP] = value; }
        }

        /// <summary>
        /// Returns the parameter set when initialized with LayerType.DATA
        /// </summary>
        public DataParameter data_param
        {
            get { return (DataParameter)m_rgLayerParameters[LayerType.DATA]; }
            set { m_rgLayerParameters[LayerType.DATA] = value; }
        }

        /// <summary>
        /// Returns the parameter set when initialized with LayerType.DEBUG
        /// </summary>
        public DebugParameter debug_param
        {
            get { return (DebugParameter)m_rgLayerParameters[LayerType.DEBUG]; }
            set { m_rgLayerParameters[LayerType.DEBUG] = value; }
        }

        /// <summary>
        /// Returns the parameter set when initialized with LayerType.DROPOUT
        /// </summary>
        public DropoutParameter dropout_param
        {
            get { return (DropoutParameter)m_rgLayerParameters[LayerType.DROPOUT]; }
            set { m_rgLayerParameters[LayerType.DROPOUT] = value; }
        }

        /// <summary>
        /// Returns the parameter set when initialized with LayerType.DUMMYDATA
        /// </summary>
        public DummyDataParameter dummy_data_param
        {
            get { return (DummyDataParameter)m_rgLayerParameters[LayerType.DUMMYDATA]; }
            set { m_rgLayerParameters[LayerType.DUMMYDATA] = value; }
        }

        /// <summary>
        /// Returns the parameter set when initialized with LayerType.ELTWISE
        /// </summary>
        public EltwiseParameter eltwise_param
        {
            get { return (EltwiseParameter)m_rgLayerParameters[LayerType.ELTWISE]; }
            set { m_rgLayerParameters[LayerType.ELTWISE] = value; }
        }

        /// <summary>
        /// Returns the parameter set when initialized with LayerType.ELU
        /// </summary>
        public EluParameter elu_param
        {
            get { return (EluParameter)m_rgLayerParameters[LayerType.ELU]; }
            set { m_rgLayerParameters[LayerType.ELU] = value; }
        }

        /// <summary>
        /// Returns the parameter set when initialized with LayerType.EMBED
        /// </summary>
        public EmbedParameter embed_param
        {
            get { return (EmbedParameter)m_rgLayerParameters[LayerType.EMBED]; }
            set { m_rgLayerParameters[LayerType.EMBED] = value; }
        }

        /// <summary>
        /// Returns the parameter set when initialized with LayerType.EXP
        /// </summary>
        public ExpParameter exp_param
        {
            get { return (ExpParameter)m_rgLayerParameters[LayerType.EXP]; }
            set { m_rgLayerParameters[LayerType.EXP] = value; }
        }

        /// <summary>
        /// Returns the parameter set when initialized with LayerType.FLATTEN
        /// </summary>
        public FlattenParameter flatten_param
        {
            get { return (FlattenParameter)m_rgLayerParameters[LayerType.FLATTEN]; }
            set { m_rgLayerParameters[LayerType.FLATTEN] = value; }
        }

        /// <summary>
        /// Returns the parameter set when initialized with LayerType.GSL
        /// </summary>
        public GradientScaleParameter gradient_scale_param
        {
            get { return (GradientScaleParameter)m_rgLayerParameters[LayerType.GRADIENTSCALER]; }
            set { m_rgLayerParameters[LayerType.GRADIENTSCALER] = value; }
        }

        /// <summary>
        /// Returns the parameter set when initialized with LayerType.HINGE_LOSS
        /// </summary>
        public HingeLossParameter hinge_loss_param
        {
            get { return (HingeLossParameter)m_rgLayerParameters[LayerType.HINGE_LOSS]; }
            set { m_rgLayerParameters[LayerType.HINGE_LOSS] = value; }
        }

        /// <summary>
        /// Returns the parameter set when initialized with LayerType.INFOGAIN_LOSS
        /// </summary>
        public InfogainLossParameter infogain_loss_param
        {
            get { return (InfogainLossParameter)m_rgLayerParameters[LayerType.INFOGAIN_LOSS]; }
            set { m_rgLayerParameters[LayerType.INFOGAIN_LOSS] = value; }
        }

        /// <summary>
        /// Returns the parameter set when initialized with LayerType.INNERPRODUCT
        /// </summary>
        public InnerProductParameter inner_product_param
        {
            get { return (InnerProductParameter)m_rgLayerParameters[LayerType.INNERPRODUCT]; }
            set { m_rgLayerParameters[LayerType.INNERPRODUCT] = value; }
        }

        /// <summary>
        /// Returns the parameter set when initialized with LayerType.KNN
        /// </summary>
        public KnnParameter knn_param
        {
            get { return (KnnParameter)m_rgLayerParameters[LayerType.KNN]; }
            set { m_rgLayerParameters[LayerType.KNN] = value; }
        }

        /// <summary>
        /// Returns the parameter set when initialized with LayerType.LABELMAPPING
        /// </summary>
        public LabelMappingParameter labelmapping_param
        {
            get { return (LabelMappingParameter)m_rgLayerParameters[LayerType.LABELMAPPING]; }
            set { m_rgLayerParameters[LayerType.LABELMAPPING] = value; }
        }

        /// <summary>
        /// Returns the parameter set when initialized with LayerType.LOG
        /// </summary>
        public LogParameter log_param
        {
            get { return (LogParameter)m_rgLayerParameters[LayerType.LOG]; }
            set { m_rgLayerParameters[LayerType.LOG] = value; }
        }

        /// <summary>
        /// Returns the parameter set when initialized with LayerType.LRN
        /// </summary>
        public LRNParameter lrn_param
        {
            get { return (LRNParameter)m_rgLayerParameters[LayerType.LRN]; }
            set { m_rgLayerParameters[LayerType.LRN] = value; }
        }

        /// <summary>
        /// Returns the parameter set when initialized with LayerType.MEMORY_DATA
        /// </summary>
        public MemoryDataParameter memory_data_param
        {
            get { return (MemoryDataParameter)m_rgLayerParameters[LayerType.MEMORYDATA]; }
            set { m_rgLayerParameters[LayerType.MEMORYDATA] = value; }
        }

        /// <summary>
        /// Returns the parameter set when initialized with LayerType.MVN
        /// </summary>
        public MVNParameter mvn_param
        {
            get { return (MVNParameter)m_rgLayerParameters[LayerType.MVN]; }
            set { m_rgLayerParameters[LayerType.MVN] = value; }
        }

        /// <summary>
        /// Returns the parameter set when initialized with LayerType.NORMALIZATION
        /// </summary>
        public NormalizationParameter normalization_param
        {
            get { return (NormalizationParameter)m_rgLayerParameters[LayerType.NORMALIZATION]; }
            set { m_rgLayerParameters[LayerType.NORMALIZATION] = value; }
        }

        /// <summary>
        /// Returns the parameter set when initialized with LayerType.POOLING
        /// </summary>
        public PoolingParameter pooling_param
        {
            get { return (PoolingParameter)m_rgLayerParameters[LayerType.POOLING]; }
            set { m_rgLayerParameters[LayerType.POOLING] = value; }
        }

        /// <summary>
        /// Returns the parameter set when initialized with LayerType.POWER
        /// </summary>
        public PowerParameter power_param
        {
            get { return (PowerParameter)m_rgLayerParameters[LayerType.POWER]; }
            set { m_rgLayerParameters[LayerType.POWER] = value; }
        }

        /// <summary>
        /// Returns the parameter set when initialized with LayerType.PRELU
        /// </summary>
        public PReLUParameter prelu_param
        {
            get { return (PReLUParameter)m_rgLayerParameters[LayerType.PRELU]; }
            set { m_rgLayerParameters[LayerType.PRELU] = value; }
        }

        /// <summary>
        /// Returns the parameter set when initialized with LayerType.REDUCTION
        /// </summary>
        public ReductionParameter reduction_param
        {
            get { return (ReductionParameter)m_rgLayerParameters[LayerType.REDUCTION]; }
            set { m_rgLayerParameters[LayerType.REDUCTION] = value; }
        }

        /// <summary>
        /// Returns the parameter set when initialized with LayerType.REINFORCEMENT_LOSS
        /// </summary>
        public ReinforcementLossParameter reinforcement_loss_param
        {
            get { return (ReinforcementLossParameter)m_rgLayerParameters[LayerType.REINFORCEMENT_LOSS]; }
            set { m_rgLayerParameters[LayerType.REINFORCEMENT_LOSS] = value; }
        }

        /// <summary>
        /// Returns the parameter set when initialized with LayerType.RELU
        /// </summary>
        public ReLUParameter relu_param
        {
            get { return (ReLUParameter)m_rgLayerParameters[LayerType.RELU]; }
            set { m_rgLayerParameters[LayerType.RELU] = value; }
        }

        /// <summary>
        /// Returns the parameter set when initialized with LayerType.RESHAPE
        /// </summary>
        public ReshapeParameter reshape_param
        {
            get { return (ReshapeParameter)m_rgLayerParameters[LayerType.RESHAPE]; }
            set { m_rgLayerParameters[LayerType.RESHAPE] = value; }
        }

        /// <summary>
        /// Returns the parameter set when initialized with LayerType.SCALE
        /// </summary>
        public ScaleParameter scale_param
        {
            get { return (ScaleParameter)m_rgLayerParameters[LayerType.SCALE]; }
            set { m_rgLayerParameters[LayerType.SCALE] = value; }
        }

        /// <summary>
        /// Returns the parameter set when initialized with LayerType.SIGMOID
        /// </summary>
        public SigmoidParameter sigmoid_param
        {
            get { return (SigmoidParameter)m_rgLayerParameters[LayerType.SIGMOID]; }
            set { m_rgLayerParameters[LayerType.SIGMOID] = value; }
        }

        /// <summary>
        /// Returns the parameter set when initialized with LayerType.SOFTMAX
        /// </summary>
        public SoftmaxParameter softmax_param
        {
            get { return (SoftmaxParameter)m_rgLayerParameters[LayerType.SOFTMAX]; }
            set { m_rgLayerParameters[LayerType.SOFTMAX] = value; }
        }

        /// <summary>
        /// Returns the parameter set when initialized with LayerType.SPP
        /// </summary>
        public SPPParameter spp_param
        {
            get { return (SPPParameter)m_rgLayerParameters[LayerType.SPP]; }
            set { m_rgLayerParameters[LayerType.SPP] = value; }
        }

        /// <summary>
        /// Returns the parameter set when initialized with LayerType.SLICE
        /// </summary>
        public SliceParameter slice_param
        {
            get { return (SliceParameter)m_rgLayerParameters[LayerType.SLICE]; }
            set { m_rgLayerParameters[LayerType.SLICE] = value; }
        }


        /// <summary>
        /// Returns the parameter set when initialized with LayerType.SWISH
        /// </summary>
        public SwishParameter swish_param
        {
            get { return (SwishParameter)m_rgLayerParameters[LayerType.SWISH]; }
            set { m_rgLayerParameters[LayerType.SWISH] = value; }
        }

        /// <summary>
        /// Returns the parameter set when initialized with LayerType.TANH
        /// </summary>
        public TanhParameter tanh_param
        {
            get { return (TanhParameter)m_rgLayerParameters[LayerType.TANH]; }
            set { m_rgLayerParameters[LayerType.TANH] = value; }
        }

        /// <summary>
        /// Returns the parameter set when initialized with LayerType.THRESHOLD
        /// </summary>
        public ThresholdParameter threshold_param
        {
            get { return (ThresholdParameter)m_rgLayerParameters[LayerType.THRESHOLD]; }
            set { m_rgLayerParameters[LayerType.THRESHOLD] = value; }
        }

        /// <summary>
        /// Returns the parameter set when initialized with LayerType.TILE
        /// </summary>
        public TileParameter tile_param
        {
            get { return (TileParameter)m_rgLayerParameters[LayerType.TILE]; }
            set { m_rgLayerParameters[LayerType.TILE] = value; }
        }

        /// <summary>
        /// Returns the parameter set when initialized with LayerType.TRIPLET_LOSS
        /// </summary>
        public TripletLossParameter triplet_loss_param
        {
            get { return (TripletLossParameter)m_rgLayerParameters[LayerType.TRIPLET_LOSS]; }
            set { m_rgLayerParameters[LayerType.TRIPLET_LOSS] = value; }
        }

        /// <summary>
        /// Returns the parameter set when initialized with LayerType.TRIPLET_LOSS_SIMPLE
        /// </summary>
        public TripletLossSimpleParameter triplet_loss_simple_param
        {
            get { return (TripletLossSimpleParameter)m_rgLayerParameters[LayerType.TRIPLET_LOSS_SIMPLE]; }
            set { m_rgLayerParameters[LayerType.TRIPLET_LOSS_SIMPLE] = value; }
        }

        /// <summary>
        /// Returns the parameter set when initialized with LayerType.LSTM_SIMPLE
        /// </summary>
        public LSTMSimpleParameter lstm_simple_param
        {
            get { return (LSTMSimpleParameter)m_rgLayerParameters[LayerType.LSTM_SIMPLE]; }
            set { m_rgLayerParameters[LayerType.LSTM_SIMPLE] = value; }
        }

        /// <summary>
        /// Returns the parameter set when initialized with LayerType.RECURRENT
        /// </summary>
        public RecurrentParameter recurrent_param
        {
            get { return (RecurrentParameter)m_rgLayerParameters[LayerType.RECURRENT]; }
            set { m_rgLayerParameters[LayerType.RECURRENT] = value; }
        }

        /// <summary>
        /// Returns the parameter set when initialized with LayerType.INPUT
        /// </summary>
        public InputParameter input_param
        {
            get { return (InputParameter)m_rgLayerParameters[LayerType.INPUT]; }
            set { m_rgLayerParameters[LayerType.INPUT] = value; }
        }

        /// <summary>
        /// Clears the collection of Blobs used by this layer.
        /// </summary>
        public void clear_blobs()
        {
            m_rgBlobs.Clear();
        }

        /// <summary>
        /// Returns the number of Solvers participating in a multi-GPU session for which the Solver using this LayerParameter is associated.
        /// </summary>
        public int solver_count
        {
            get { return m_nSolverCount; }
            set { m_nSolverCount = value; }
        }

        /// <summary>
        /// Returns the SolverRank of the Solver using this LayerParameter (if any).
        /// </summary>
        public int solver_rank
        {
            get { return m_nSolverRank; }
            set { m_nSolverRank = value; }
        }

        /// <summary>
        /// Returns a list of <i>expected</i> top connections (in the bottom, out the top).
        /// </summary>
        public List<string> expected_top
        {
            get { return m_rgstrExpectedTop; }
        }

        /// <summary>
        /// Returns a list of <i>expected</i> bottom connections (in the bottom, out the top).
        /// </summary>
        public List<string> expected_bottom
        {
            get { return m_rgstrExpectedBottom; }
        }

        /// <summary>
        /// Copy just the layer specific parameters to this layer parameter.
        /// </summary>
        /// <param name="src">Specifies the source who's specific layer parameters are to be compied.</param>
        public void CopyParameters(LayerParameter src)
        {
            m_rgLayerParameters = new Dictionary<LayerType, LayerParameterBase>();

            foreach (KeyValuePair<LayerType, LayerParameterBase> kv in src.m_rgLayerParameters)
            {
                if (kv.Value != null)
                    m_rgLayerParameters.Add(kv.Key, kv.Value.Clone());
                else
                    m_rgLayerParameters.Add(kv.Key, null);
            }
        }

        /// <summary>
        /// Creates a new copy of this instance of the parameter.
        /// </summary>
        /// <returns>A new instance of this parameter is returned.</returns>
        public virtual LayerParameter Clone(bool bCloneBlobs)
        {
            LayerParameter p = new LayerParameter(m_type, m_strName);

            p.m_rgstrBottom = Utility.Clone<string>(m_rgstrBottom);
            p.m_rgstrTop = Utility.Clone<string>(m_rgstrTop);
            p.m_phase = m_phase;
            p.m_rgLossWeight = Utility.Clone<double>(m_rgLossWeight);
            p.m_rgParams = Utility.Clone<ParamSpec>(m_rgParams);

            if (bCloneBlobs)
                p.m_rgBlobs = Utility.Clone<BlobProto>(m_rgBlobs);

            p.m_rgbPropagateDown = Utility.Clone<bool>(m_rgbPropagateDown);
            p.m_rgInclude = Utility.Clone<NetStateRule>(m_rgInclude);
            p.m_rgExclude = Utility.Clone<NetStateRule>(m_rgExclude);

            p.m_rgLayerParameters = new Dictionary<LayerType, LayerParameterBase>();

            foreach (KeyValuePair<LayerType, LayerParameterBase> kv in m_rgLayerParameters)
            {
                if (kv.Value != null)
                    p.m_rgLayerParameters.Add(kv.Key, kv.Value.Clone());
                else
                    p.m_rgLayerParameters.Add(kv.Key, null);
            }

            p.m_nSolverCount = m_nSolverCount;
            p.m_nSolverRank = m_nSolverRank;

            return p;
        }

        /// <summary>
        /// Creates a new copy of this instance of the parameter.
        /// </summary>
        /// <returns>A new instance of this parameter is returned.</returns>
        object ICloneable.Clone()
        {
            return Clone(true);
        }

        /** @copydoc BaseParameter */
        public int CompareTo(object obj)
        {
            LayerParameter p = obj as LayerParameter;

            if (p == null)
                return 1;

            if (!Compare(p))
                return 1;

            return 0;
        }

        private string getTypeString(LayerType type)
        {
            switch (type)
            {
                case LayerType.ABSVAL:
                    return "AbsVal";

                case LayerType.ACCURACY:
                    return "Accuracy";

                case LayerType.ARGMAX:
                    return "ArgMax";

                case LayerType.BATCHDATA:
                    return "BatchData";

                case LayerType.BATCHNORM:
                    return "BatchNorm";

                case LayerType.BATCHREINDEX:
                    return "BatchReIndex";

                case LayerType.BIAS:
                    return "Bias";

                case LayerType.BINARYHASH:
                    return "BinaryHash";

                case LayerType.BNLL:
                    return "BNLL";

                case LayerType.CONCAT:
                    return "Concat";

                case LayerType.CONTRASTIVE_LOSS:
                    return "ContrastiveLoss";

                case LayerType.CONVOLUTION:
                    return "Convolution";

                case LayerType.CROP:
                    return "Crop";

                case LayerType.DATA:
                    return "Data";

                case LayerType.DEBUG:
                    return "Debug";

                case LayerType.DECONVOLUTION:
                    return "Deconvolution";

                case LayerType.DROPOUT:
                    return "Dropout";

                case LayerType.DUMMYDATA:
                    return "DummyData";

                case LayerType.ELTWISE:
                    return "Eltwise";

                case LayerType.ELU:
                    return "ELU";

                case LayerType.EMBED:
                    return "Embed";

                case LayerType.EUCLIDEAN_LOSS:
                    return "EuclideanLoss";

                case LayerType.EXP:
                    return "EXP";

                case LayerType.FILTER:
                    return "Filter";

                case LayerType.FLATTEN:
                    return "Flatten";

                case LayerType.GRN:
                    return "GRN";

                case LayerType.GRADIENTSCALER:
                    return "GSL";

                case LayerType.HINGE_LOSS:
                    return "HingeLoss";

                case LayerType.IM2COL:
                    return "Im2Col";

                case LayerType.INFOGAIN_LOSS:
                    return "InfogainLoss";

                case LayerType.INNERPRODUCT:
                    return "InnerProduct";

                case LayerType.KNN:
                    return "Knn";

                case LayerType.LABELMAPPING:
                    return "LabelMapping";

                case LayerType.LOG:
                    return "Log";

                case LayerType.LOSS:
                    return "Loss";

                case LayerType.LRN:
                    return "LRN";

                case LayerType.MEMORYDATA:
                    return "MemoryData";

                case LayerType.MULTINOMIALLOGISTIC_LOSS:
                    return "MultinomialLogisticLoss";

                case LayerType.MVN:
                    return "MVN";

                case LayerType.NORMALIZATION:
                    return "Normalization";

                case LayerType.POOLING:
                    return "Pooling";

                case LayerType.UNPOOLING1:
                    return "UnPooling1";

                case LayerType.UNPOOLING2:
                    return "UnPooling2";

                case LayerType.POWER:
                    return "Power";

                case LayerType.PRELU:
                    return "PReLU";

                case LayerType.REDUCTION:
                    return "Reduction";

                case LayerType.REINFORCEMENT_LOSS:
                    return "ReinforcementLoss";

                case LayerType.RELU:
                    return "ReLU";

                case LayerType.RESHAPE:
                    return "Reshape";

                case LayerType.SCALE:
                    return "Scale";

                case LayerType.SIGMOID:
                    return "Sigmoid";

                case LayerType.SIGMOIDCROSSENTROPY_LOSS:
                    return "SigmoidCrossEntropyLoss";

                case LayerType.SILENCE:
                    return "Silence";

                case LayerType.SLICE:
                    return "Slice";

                case LayerType.SOFTMAX:
                    return "Softmax";

                case LayerType.SOFTMAXWITH_LOSS:
                    return "SoftmaxWithLoss";

                case LayerType.SPLIT:
                    return "Split";

                case LayerType.SPP:
                    return "SPP";

                case LayerType.SWISH:
                    return "Swish";

                case LayerType.TANH:
                    return "TanH";

                case LayerType.THRESHOLD:
                    return "Threshold";

                case LayerType.TILE:
                    return "Tile";

                case LayerType.TRIPLET_LOSS_SIMPLE:
                    return "SimpleTripletLoss";

                case LayerType.TRIPLET_LOSS:
                    return "TripletLoss";

                case LayerType.TRIPLET_SELECT:
                    return "TripletSelection";

                case LayerType.TRIPLET_DATA:
                    return "TripletData";

                case LayerType.LSTM_SIMPLE:
                    return "LstmSimple";

                case LayerType.RNN:
                    return "Rnn";

                case LayerType.LSTM:
                    return "Lstm";

                case LayerType.INPUT:
                    return "Input";

                default:
                    return "Unknown";
            }
        }

        /** @copydoc BaseParameter */
        public override RawProto ToProto(string strName)
        {
            RawProtoCollection rgChildren = new RawProtoCollection();

            rgChildren.Add("name", name, RawProto.TYPE.STRING);
            rgChildren.Add("type", getTypeString(type), RawProto.TYPE.STRING);
            rgChildren.Add<string>("bottom", bottom);
            rgChildren.Add<string>("top", top);
            rgChildren.Add<double>("loss_weight", loss_weight);

            foreach (ParamSpec ps in parameters)
            {
                rgChildren.Add(ps.ToProto("param"));
            }

            foreach (BlobProto bp in blobs)
            {
                rgChildren.Add(bp.ToProto("blobs"));
            }

            rgChildren.Add<bool>("propagate_down", propagate_down);

            foreach (NetStateRule nsr in include)
            {
                rgChildren.Add(nsr.ToProto("include"));
            }

            foreach (NetStateRule nsr in exclude)
            {
                rgChildren.Add(nsr.ToProto("exclude"));
            }

            foreach (KeyValuePair<Phase, int> kv in m_rgMaxBottomCount)
            {
                RawProtoCollection prChildren = new RawProtoCollection();
                prChildren.Add("phase", kv.Key.ToString());
                prChildren.Add("count", kv.Value.ToString());
                RawProto prMaxBottomCount = new RawProto("max_bottom_count", "", prChildren);
                rgChildren.Add(prMaxBottomCount);
            }

            List<KeyValuePair<BaseParameter, string>> rgParam = new List<KeyValuePair<BaseParameter,string>>();

            rgParam.Add(new KeyValuePair<BaseParameter,string>(transform_param, "transform_param"));
            rgParam.Add(new KeyValuePair<BaseParameter,string>(loss_param, "loss_param"));
            rgParam.Add(new KeyValuePair<BaseParameter,string>(accuracy_param, "accuracy_param"));
            rgParam.Add(new KeyValuePair<BaseParameter, string>(argmax_param, "argmax_param"));
            rgParam.Add(new KeyValuePair<BaseParameter, string>(batch_data_param, "batch_data_param"));
            rgParam.Add(new KeyValuePair<BaseParameter, string>(batch_norm_param, "batch_norm_param"));
            rgParam.Add(new KeyValuePair<BaseParameter, string>(bias_param, "bias_param"));
            rgParam.Add(new KeyValuePair<BaseParameter, string>(binary_hash_param, "binaryhash_param"));
            rgParam.Add(new KeyValuePair<BaseParameter, string>(concat_param, "concat_param"));
            rgParam.Add(new KeyValuePair<BaseParameter, string>(contrastive_loss_param, "contrastive_loss_param"));
            rgParam.Add(new KeyValuePair<BaseParameter, string>(convolution_param, "convolution_param"));
            rgParam.Add(new KeyValuePair<BaseParameter, string>(data_param, "data_param"));
            rgParam.Add(new KeyValuePair<BaseParameter, string>(debug_param, "debug_param"));
            rgParam.Add(new KeyValuePair<BaseParameter, string>(dropout_param, "dropout_param"));
            rgParam.Add(new KeyValuePair<BaseParameter, string>(dummy_data_param, "dummy_data_param"));
            rgParam.Add(new KeyValuePair<BaseParameter, string>(eltwise_param, "eltwise_param"));
            rgParam.Add(new KeyValuePair<BaseParameter, string>(elu_param, "elu_param"));
            rgParam.Add(new KeyValuePair<BaseParameter, string>(embed_param, "embed_param"));
            rgParam.Add(new KeyValuePair<BaseParameter, string>(exp_param, "exp_param"));
            rgParam.Add(new KeyValuePair<BaseParameter, string>(flatten_param, "flatten_param"));
            rgParam.Add(new KeyValuePair<BaseParameter, string>(gradient_scale_param, "gradient_scale_param"));
            rgParam.Add(new KeyValuePair<BaseParameter, string>(hinge_loss_param, "hinge_loss_param"));
            rgParam.Add(new KeyValuePair<BaseParameter, string>(infogain_loss_param, "infogain_loss_param"));
            rgParam.Add(new KeyValuePair<BaseParameter, string>(inner_product_param, "inner_product_param"));
            rgParam.Add(new KeyValuePair<BaseParameter, string>(knn_param, "knn_param"));
            rgParam.Add(new KeyValuePair<BaseParameter, string>(labelmapping_param, "labelmapping_param"));
            rgParam.Add(new KeyValuePair<BaseParameter, string>(log_param, "log_param"));
            rgParam.Add(new KeyValuePair<BaseParameter, string>(lrn_param, "lrn_param"));
            rgParam.Add(new KeyValuePair<BaseParameter, string>(memory_data_param, "memory_data_param"));
            rgParam.Add(new KeyValuePair<BaseParameter, string>(mvn_param, "mvn_param"));
            rgParam.Add(new KeyValuePair<BaseParameter, string>(normalization_param, "normalization_param"));
            rgParam.Add(new KeyValuePair<BaseParameter, string>(pooling_param, "pooling_param"));
            rgParam.Add(new KeyValuePair<BaseParameter, string>(power_param, "power_param"));
            rgParam.Add(new KeyValuePair<BaseParameter, string>(prelu_param, "prelu_param"));
            rgParam.Add(new KeyValuePair<BaseParameter, string>(reduction_param, "reduction_param"));
            rgParam.Add(new KeyValuePair<BaseParameter, string>(reinforcement_loss_param, "reinforcement_loss_param"));
            rgParam.Add(new KeyValuePair<BaseParameter, string>(relu_param, "relu_param"));
            rgParam.Add(new KeyValuePair<BaseParameter, string>(reshape_param, "reshape_param"));
            rgParam.Add(new KeyValuePair<BaseParameter, string>(scale_param, "scale_param"));
            rgParam.Add(new KeyValuePair<BaseParameter, string>(sigmoid_param, "sigmoid_param"));
            rgParam.Add(new KeyValuePair<BaseParameter, string>(softmax_param, "softmax_param"));
            rgParam.Add(new KeyValuePair<BaseParameter, string>(spp_param, "spp_param"));
            rgParam.Add(new KeyValuePair<BaseParameter, string>(slice_param, "slice_param"));
            rgParam.Add(new KeyValuePair<BaseParameter, string>(swish_param, "swish_param"));
            rgParam.Add(new KeyValuePair<BaseParameter, string>(tanh_param, "tanh_param"));
            rgParam.Add(new KeyValuePair<BaseParameter, string>(threshold_param, "threshold_param"));
            rgParam.Add(new KeyValuePair<BaseParameter, string>(tile_param, "tile_param"));
            rgParam.Add(new KeyValuePair<BaseParameter, string>(triplet_loss_param, "triplet_loss_param"));
            rgParam.Add(new KeyValuePair<BaseParameter, string>(triplet_loss_simple_param, "triplet_loss_simple_param"));
            rgParam.Add(new KeyValuePair<BaseParameter, string>(lstm_simple_param, "lstm_simple_param"));
            rgParam.Add(new KeyValuePair<BaseParameter, string>(recurrent_param, "recurrent_param"));

            foreach (KeyValuePair<BaseParameter, string> kv in rgParam)
            {
                if (kv.Key != null)
                    rgChildren.Add(kv.Key.ToProto(kv.Value));
            }

            return new RawProto(strName, "", rgChildren);
        }

        /// <summary>
        /// Parses the parameter from a RawProto.
        /// </summary>
        /// <param name="rp">Specifies the RawProto to parse.</param>
        /// <returns>A new instance of the parameter is returned.</returns>
        public static LayerParameter FromProto(RawProto rp)
        {
            string strVal;
            string strName = null;
            LayerType layerType;

            if ((strVal = rp.FindValue("name")) != null)
                strName = strVal;

            if ((strVal = rp.FindValue("type")) == null)
                throw new Exception("No layer type specified!");

            layerType = parseLayerType(strVal);

            LayerParameter p = new LayerParameter(layerType, strName);

            p.bottom = rp.FindArray<string>("bottom");
            p.top = rp.FindArray<string>("top");

            if ((strVal = rp.FindValue("phase")) != null)
                p.phase = parsePhase(strVal);

            p.loss_weight = rp.FindArray<double>("loss_weight");

            RawProtoCollection rgrp;

            rgrp = rp.FindChildren("param");
            foreach (RawProto rpChild in rgrp)
            {
                p.parameters.Add(ParamSpec.FromProto(rpChild));
            }

            rgrp = rp.FindChildren("blobs");
            foreach (RawProto rpChild in rgrp)
            {
                p.blobs.Add(BlobProto.FromProto(rpChild));
            }

            p.propagate_down = rp.FindArray<bool>("propagate_down");

            rgrp = rp.FindChildren("include");
            foreach (RawProto rpChild in rgrp)
            {
                p.include.Add(NetStateRule.FromProto(rpChild));
            }

            rgrp = rp.FindChildren("exclude");
            foreach (RawProto rpChild in rgrp)
            {
                p.exclude.Add(NetStateRule.FromProto(rpChild));
            }

            rgrp = rp.FindChildren("max_bottom_count");
            foreach (RawProto rpChild in rgrp)
            {
                RawProto prPhase = rpChild.FindChild("phase");
                if (prPhase != null)
                {
                    Phase phase = parsePhase(prPhase.Value);
                    if (!p.m_rgMaxBottomCount.ContainsKey(phase))
                    {
                        RawProto prCount = rpChild.FindChild("count");
                        if (prCount != null)
                            p.m_rgMaxBottomCount.Add(phase, int.Parse(prCount.Value));
                    }
                }
            }

            RawProto rpp;

            if ((rpp = rp.FindChild("transform_param")) != null)
                p.transform_param = TransformationParameter.FromProto(rpp);

            if ((rpp = rp.FindChild("loss_param")) != null)
                p.loss_param = LossParameter.FromProto(rpp);

            if ((rpp = rp.FindChild("accuracy_param")) != null)
                p.accuracy_param = AccuracyParameter.FromProto(rpp);

            if ((rpp = rp.FindChild("argmax_param")) != null)
                p.argmax_param = ArgMaxParameter.FromProto(rpp);

            if ((rpp = rp.FindChild("batch_data_param")) != null)
                p.batch_data_param = BatchDataParameter.FromProto(rpp);

            if ((rpp = rp.FindChild("batch_norm_param")) != null)
                p.batch_norm_param = BatchNormParameter.FromProto(rpp);

            if ((rpp = rp.FindChild("bias_param")) != null)
                p.bias_param = BiasParameter.FromProto(rpp);

            if ((rpp = rp.FindChild("binaryhash_param")) != null)
                p.binary_hash_param = BinaryHashParameter.FromProto(rpp);

            if ((rpp = rp.FindChild("concat_param")) != null)
                p.concat_param = ConcatParameter.FromProto(rpp);

            if ((rpp = rp.FindChild("contrastive_loss_param")) != null)
                p.contrastive_loss_param = ContrastiveLossParameter.FromProto(rpp);

            if ((rpp = rp.FindChild("convolution_param")) != null)
                p.convolution_param = ConvolutionParameter.FromProto(rpp);

            if ((rpp = rp.FindChild("data_param")) != null)
                p.data_param = DataParameter.FromProto(rpp);

            if ((rpp = rp.FindChild("debug_param")) != null)
                p.debug_param = DebugParameter.FromProto(rpp);

            if ((rpp = rp.FindChild("dropout_param")) != null)
                p.dropout_param = DropoutParameter.FromProto(rpp);

            if ((rpp = rp.FindChild("dummy_data_param")) != null)
                p.dummy_data_param = DummyDataParameter.FromProto(rpp);

            if ((rpp = rp.FindChild("eltwise_param")) != null)
                p.eltwise_param = EltwiseParameter.FromProto(rpp);

            if ((rpp = rp.FindChild("elu_param")) != null)
                p.elu_param = EluParameter.FromProto(rpp);

            if ((rpp = rp.FindChild("embed_param")) != null)
                p.embed_param = EmbedParameter.FromProto(rpp);

            if ((rpp = rp.FindChild("exp_param")) != null)
                p.exp_param = ExpParameter.FromProto(rpp);

            if ((rpp = rp.FindChild("flatten_param")) != null)
                p.flatten_param = FlattenParameter.FromProto(rpp);

            if ((rpp = rp.FindChild("gradient_scale_param")) != null)
                p.gradient_scale_param = GradientScaleParameter.FromProto(rpp);

            if ((rpp = rp.FindChild("hinge_loss_param")) != null)
                p.hinge_loss_param = HingeLossParameter.FromProto(rpp);

            if ((rpp = rp.FindChild("infogain_loss_param")) != null)
                p.infogain_loss_param = InfogainLossParameter.FromProto(rpp);

            if ((rpp = rp.FindChild("inner_product_param")) != null)
                p.inner_product_param = InnerProductParameter.FromProto(rpp);

            if ((rpp = rp.FindChild("knn_param")) != null)
                p.knn_param = KnnParameter.FromProto(rpp);

            if ((rpp = rp.FindChild("labelmapping_param")) != null)
                p.labelmapping_param = LabelMappingParameter.FromProto(rpp);

            if ((rpp = rp.FindChild("log_param")) != null)
                p.log_param = LogParameter.FromProto(rpp);

            if ((rpp = rp.FindChild("lrn_param")) != null)
                p.lrn_param = LRNParameter.FromProto(rpp);

            if ((rpp = rp.FindChild("memory_data_param")) != null)
                p.memory_data_param = MemoryDataParameter.FromProto(rpp);

            if ((rpp = rp.FindChild("mvn_param")) != null)
                p.mvn_param = MVNParameter.FromProto(rpp);

            if ((rpp = rp.FindChild("normalization_param")) != null)
                p.normalization_param = NormalizationParameter.FromProto(rpp);

            if ((rpp = rp.FindChild("pooling_param")) != null)
                p.pooling_param = PoolingParameter.FromProto(rpp);

            if ((rpp = rp.FindChild("power_param")) != null)
                p.power_param = PowerParameter.FromProto(rpp);

            if ((rpp = rp.FindChild("prelu_param")) != null)
                p.prelu_param = PReLUParameter.FromProto(rpp);

            if ((rpp = rp.FindChild("reduction_param")) != null)
                p.reduction_param = ReductionParameter.FromProto(rpp);

            if ((rpp = rp.FindChild("reinforcement_loss_param")) != null)
                p.reinforcement_loss_param = ReinforcementLossParameter.FromProto(rpp);

            if ((rpp = rp.FindChild("relu_param")) != null)
                p.relu_param = ReLUParameter.FromProto(rpp);

            if ((rpp = rp.FindChild("reshape_param")) != null)
                p.reshape_param = ReshapeParameter.FromProto(rpp);

            if ((rpp = rp.FindChild("scale_param")) != null)
                p.scale_param = ScaleParameter.FromProto(rpp);

            if ((rpp = rp.FindChild("sigmoid_param")) != null)
                p.sigmoid_param = SigmoidParameter.FromProto(rpp);

            if ((rpp = rp.FindChild("softmax_param")) != null)
                p.softmax_param = SoftmaxParameter.FromProto(rpp);

            if ((rpp = rp.FindChild("spp_param")) != null)
                p.spp_param = SPPParameter.FromProto(rpp);

            if ((rpp = rp.FindChild("slice_param")) != null)
                p.slice_param = SliceParameter.FromProto(rpp);

            if ((rpp = rp.FindChild("swish_param")) != null)
                p.swish_param = SwishParameter.FromProto(rpp);

            if ((rpp = rp.FindChild("tanh_param")) != null)
                p.tanh_param = TanhParameter.FromProto(rpp);

            if ((rpp = rp.FindChild("threshold_param")) != null)
                p.threshold_param = ThresholdParameter.FromProto(rpp);

            if ((rpp = rp.FindChild("tile_param")) != null)
                p.tile_param = TileParameter.FromProto(rpp);

            if ((rpp = rp.FindChild("triplet_loss_param")) != null)
                p.triplet_loss_param = TripletLossParameter.FromProto(rpp);

            if ((rpp = rp.FindChild("triplet_loss_simple_param")) != null)
                p.triplet_loss_simple_param = TripletLossSimpleParameter.FromProto(rpp);

            if ((rpp = rp.FindChild("lstm_simple_param")) != null)
                p.lstm_simple_param = LSTMSimpleParameter.FromProto(rpp);

            if ((rpp = rp.FindChild("recurrent_param")) != null)
                p.recurrent_param = RecurrentParameter.FromProto(rpp);
            
            return p;
        }

        private static Phase parsePhase(string strVal)
        {
            switch (strVal)
            {
                case "TEST":
                    return Phase.TEST;

                case "TRAIN":
                    return Phase.TRAIN;

                case "RUN":
                    return Phase.RUN;

                case "NONE":
                    return Phase.NONE;

                default:
                    throw new Exception("Unknown 'phase' value: " + strVal);
            }
        }

        private static LayerType parseLayerType(string str)
        {
            str = str.ToLower();

            switch (str)
            {
                case "absval":
                    return LayerType.ABSVAL;

                case "accuracy":
                    return LayerType.ACCURACY;

                case "argmax":
                    return LayerType.ARGMAX;

                case "batchdata":
                    return LayerType.BATCHDATA;

                case "batchnorm":
                    return LayerType.BATCHNORM;

                case "batchreindex":
                    return LayerType.BATCHREINDEX;

                case "bias":
                    return LayerType.BIAS;

                case "binaryhash":
                    return LayerType.BINARYHASH;

                case "bnll":
                    return LayerType.BNLL;

                case "concat":
                    return LayerType.CONCAT;

                case "contrastiveloss":
                case "contrastive_loss":
                    return LayerType.CONTRASTIVE_LOSS;

                case "convolution":
                    return LayerType.CONVOLUTION;

                case "crop":
                    return LayerType.CROP;

                case "data":
                    return LayerType.DATA;

                case "debug":
                    return LayerType.DEBUG;

                case "deconvolution":
                    return LayerType.DECONVOLUTION;

                case "dropout":
                    return LayerType.DROPOUT;

                case "dummydata":
                    return LayerType.DUMMYDATA;

                case "eltwise":
                    return LayerType.ELTWISE;

                case "elu":
                    return LayerType.ELU;

                case "embed":
                    return LayerType.EMBED;

                case "euclideanloss":
                case "euclidean_loss":
                    return LayerType.EUCLIDEAN_LOSS;

                case "exp":
                    return LayerType.EXP;

                case "filter":
                    return LayerType.FILTER;

                case "flatten":
                    return LayerType.FLATTEN;

                case "grn":
                    return LayerType.GRN;

                case "gsl":
                    return LayerType.GRADIENTSCALER;

//                case "hdf5data":
//                    return LayerType.HDF5DATA;

//                case "hdf5output":
//                    return LayerType.HDF5OUTPUT;

                case "hingeloss":
                case "hinge_loss":
                    return LayerType.HINGE_LOSS;

                case "im2col":
                    return LayerType.IM2COL;

//                case "imagedata":
//                    return LayerType.IMAGEDATA;

                case "infogainloss":
                case "infogain_loss":
                    return LayerType.INFOGAIN_LOSS;

                case "innerproduct":
                case "inner_product":
                    return LayerType.INNERPRODUCT;

                case "knn":
                    return LayerType.KNN;

                case "labelmapping":
                    return LayerType.LABELMAPPING;

                case "log":
                    return LayerType.LOG;

                case "lrn":
                    return LayerType.LRN;

                case "memorydata":
                    return LayerType.MEMORYDATA;

                case "multinomiallogisticloss":
                case "multinomiallogistic_loss":
                    return LayerType.MULTINOMIALLOGISTIC_LOSS;

                case "mvn":
                    return LayerType.MVN;

                case "normalization":
                    return LayerType.NORMALIZATION;

                case "pooling":
                    return LayerType.POOLING;

                case "unpooling1":
                    return LayerType.UNPOOLING1;

                case "unpooling2":
                    return LayerType.UNPOOLING2;

                case "power":
                    return LayerType.POWER;

                case "prelu":
                    return LayerType.PRELU;

                case "reduction":
                    return LayerType.REDUCTION;

                case "reinforcementloss":
                case "reinforcement_loss":
                    return LayerType.REINFORCEMENT_LOSS;

                case "relu":
                    return LayerType.RELU;

                case "reshape":
                    return LayerType.RESHAPE;

                case "scale":
                    return LayerType.SCALE;

                case "sigmoid":
                    return LayerType.SIGMOID;

                case "sigmoidcrossentropyloss":
                case "sigmoidcrossentropy_loss":
                    return LayerType.SIGMOIDCROSSENTROPY_LOSS;

                case "silence":
                    return LayerType.SILENCE;

                case "slice":
                    return LayerType.SLICE;

                case "softmax":
                    return LayerType.SOFTMAX;

                case "softmaxwithloss":
                case "softmaxwith_loss":
                case "softmax_loss":
                    return LayerType.SOFTMAXWITH_LOSS;

                case "split":
                    return LayerType.SPLIT;

                case "spp":
                    return LayerType.SPP;

                case "swish":
                    return LayerType.SWISH;

                case "tanh":
                    return LayerType.TANH;

                case "threshold":
                    return LayerType.THRESHOLD;

                case "tile":
                    return LayerType.TILE;

                case "simple_triplet_loss":
                case "simpletripletloss":
                    return LayerType.TRIPLET_LOSS_SIMPLE;

                case "triplet_data":
                case "tripletdata":
                    return LayerType.TRIPLET_DATA;

                case "triplet_loss":
                case "tripletloss":
                    return LayerType.TRIPLET_LOSS;

                case "triplet_selection":
                case "tripletselection":
                    return LayerType.TRIPLET_SELECT;

                // case "windowdata":
                //      return LayerType.WINDOWDATA;

                case "lstmsimple":
                case "lstm_simple":
                    return LayerType.LSTM_SIMPLE;

                case "rnn":
                    return LayerType.RNN;

                case "lstm":
                    return LayerType.LSTM;

                case "input":
                    return LayerType.INPUT;

                default:
                    throw new Exception("Unknown 'layertype' value: " + str);
            }
        }

        /// <summary>
        /// Returns a string representation of the LayerParameter.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return m_strName + " (" + m_type.ToString() + ")";
        }
    }
}
