/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import { sizeMap } from './size-map'
import { IconContainer } from './styles'

const IconLeftArrow = ({ color = 'black', size = 's' }) => {
  return (
    <IconContainer color={color} w={sizeMap[size].w} h={sizeMap[size].h}>
      <svg
        width="40"
        height="40"
        viewBox="0 0 40 40"
        fill="none"
        xmlns="http://www.w3.org/2000/svg"
      >
        <path
          fillRule="evenodd"
          clipRule="evenodd"
          d="M3.33337 19.9999C3.33337 20.9204 4.07957 21.6666 5.00004 21.6666L35 21.6666C35.9205 21.6666 36.6667 20.9204 36.6667 19.9999C36.6667 19.0795 35.9205 18.3333 35 18.3333L5.00004 18.3333C4.07957 18.3333 3.33337 19.0795 3.33337 19.9999Z"
          fill="currentColor"
        />
        <path
          fillRule="evenodd"
          clipRule="evenodd"
          d="M3.82153 18.8214C3.17066 19.4723 3.17066 20.5276 3.82153 21.1784L15.4882 32.8451C16.1391 33.496 17.1943 33.496 17.8452 32.8451C18.4961 32.1943 18.4961 31.139 17.8452 30.4881L7.35706 19.9999L17.8452 9.51179C18.4961 8.86092 18.4961 7.80564 17.8452 7.15477C17.1943 6.50389 16.1391 6.50389 15.4882 7.15477L3.82153 18.8214Z"
          fill="currentColor"
        />
      </svg>
    </IconContainer>
  )
}

export default IconLeftArrow
