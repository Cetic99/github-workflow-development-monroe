import styled from '@emotion/styled'
import Image from '@ui/assets/images/parcel-locker/scan-parcel.png'

const Container = styled.div`
  position: relative;
  width: fit-content;
  width: 600px;
  height: 600px;

  & svg {
    z-index: 3;
    position: absolute;
    top: 0;
    left: 0;
  }

  & img {
    z-index: 4;
    position: absolute;
    top: 0;
    left: 0;

    width: 600px;
    height: 600px;
  }
`

const ImageScanParcel = () => {
  return (
    <Container className="image-phone-scan-qr">
      <svg
        width="761"
        height="373"
        viewBox="0 0 761 373"
        fill="none"
        xmlns="http://www.w3.org/2000/svg"
      >
        <g filter="url(#filter0_f_2843_9364)">
          <path d="M183 227H586L666.5 320H104.5L183 227Z" fill="#728396" />
        </g>
        <rect
          width="774"
          height="297"
          transform="matrix(-1 0 0 1 761 76)"
          fill="url(#pattern0_2843_9364)"
        />
        <rect
          x="309"
          y="1"
          width="93"
          height="18"
          rx="5"
          fill="#004B3E"
          stroke="black"
          strokeWidth="2"
        />
        <rect x="314" y="6" width="83" height="8" rx="2" fill="url(#paint0_linear_2843_9364)" />
        <path d="M355.5 34L522.21 189.25H188.79L355.5 34Z" fill="url(#paint1_linear_2843_9364)" />
        <defs>
          <filter
            id="filter0_f_2843_9364"
            x="64.5"
            y="187"
            width="642"
            height="173"
            filterUnits="userSpaceOnUse"
            colorInterpolationFilters="sRGB"
          >
            <feFlood floodOpacity="0" result="BackgroundImageFix" />
            <feBlend mode="normal" in="SourceGraphic" in2="BackgroundImageFix" result="shape" />
            <feGaussianBlur stdDeviation="20" result="effect1_foregroundBlur_2843_9364" />
          </filter>
          <pattern
            id="pattern0_2843_9364"
            patternContentUnits="objectBoundingBox"
            width="1"
            height="1"
          >
            <use transform="matrix(0.0005 0 0 0.00130303 0 -0.00101515)" />
          </pattern>
          <linearGradient
            id="paint0_linear_2843_9364"
            x1="355.5"
            y1="14"
            x2="355.5"
            y2="6"
            gradientUnits="userSpaceOnUse"
          >
            <stop stopColor="#004B3E" />
            <stop offset="1" stopColor="#00B192" />
          </linearGradient>
          <linearGradient
            id="paint1_linear_2843_9364"
            x1="355.5"
            y1="34"
            x2="355.5"
            y2="241"
            gradientUnits="userSpaceOnUse"
          >
            <stop stopColor="#2BD6B5" stopOpacity="0.75" />
            <stop offset="1" stopColor="#2BD6B5" stopOpacity="0" />
          </linearGradient>
          <image id="image0_2843_9364" width="2000" height="769" preserveAspectRatio="none" />
        </defs>
      </svg>

      <img src={Image} />
    </Container>
  )
}

export default ImageScanParcel
